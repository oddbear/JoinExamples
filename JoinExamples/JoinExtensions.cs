using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JoinExamples
{
    public static class JoinExtensions
    {
        public static IQueryable<QEntity> Join<QEntity, QKey, JEntity, JKey>( //LeftJoin == + .DefaultIfEmpty()
            this IQueryable<QEntity> query,
            LinqToDB.ITable<JEntity> table,
            Expression<Func<QEntity, QKey>> queryKey,
            Expression<Func<JEntity, JKey>> joinKey,
            Expression<Func<QEntity, JEntity>> resultSelectorExpression
        )
        {
            var joinPropertyInfo = ((PropertyInfo)((MemberExpression)joinKey.Body).Member);

            var queryParameter = Expression.Parameter(typeof(QEntity), "query");
            var joinCollectionParameter = Expression.Parameter(typeof(IQueryable<JEntity>), "joinCollection"); //ex. (l, r) => ...

            //join => query.ForeignKeyId == (queryType)join.Id
            var wherePredicate = CreateWherePredicate<QEntity, QKey, JEntity>(queryParameter, queryKey, joinPropertyInfo.Name);

            var whereCall = CreateWhereCall<JEntity>(joinCollectionParameter, wherePredicate);

            //query => Queryable.Where(joinCollection, join => query.ForeignKeyId == (queryType)join.Id)
            var collectionSelectorExpression = CreateCollectionSelectorExpression<QEntity, JEntity>(queryParameter, whereCall);

            //var resultBinding = Bind(resultSelectorExpression);
            //Bind<User, Street>((u, s) => new User { Address = new Address { Street = s }});

            //resultSelectorExpression.ToString().Dump();
            //resultBinding.ToString().Dump();

            //(queryCollection, joinCollection) =>
            //	Queryable.SelectMany(queryCollection, query =>
            //		Queryable.Where(joinCollection, join => query.ForeignKeyId == (queryType)join.Id),
            //	(i, j) => i);
            var expression = CreateExpression(joinCollectionParameter, collectionSelectorExpression, Bind(resultSelectorExpression));

            //expression.Dump(1);
            return expression.Compile()(query, table);
        }

        public static IQueryable<QEntity> LeftJoin<QEntity, QKey, JEntity, JKey>( //LeftJoin == + .DefaultIfEmpty()
            this IQueryable<QEntity> query,
            LinqToDB.ITable<JEntity> table,
            Expression<Func<QEntity, QKey>> queryKey,
            Expression<Func<JEntity, JKey>> joinKey,
            Expression<Func<QEntity, JEntity>> resultSelectorExpression
        )
        {
            var joinPropertyInfo = ((PropertyInfo)((MemberExpression)joinKey.Body).Member);

            var queryParameter = Expression.Parameter(typeof(QEntity), "query");
            var joinCollectionParameter = Expression.Parameter(typeof(IQueryable<JEntity>), "joinCollection"); //ex. (l, r) => ...

            //join => query.ForeignKeyId == (queryType)join.Id
            var wherePredicate = CreateWherePredicate<QEntity, QKey, JEntity>(queryParameter, queryKey, joinPropertyInfo.Name);

            var whereCall = CreateWhereCall<JEntity>(joinCollectionParameter, wherePredicate);

            var defaultIfEmptyMethodInfo = GetDefaultIfEmpty();
            var genericDefaultIfEmptyMethodInfo = defaultIfEmptyMethodInfo.MakeGenericMethod(typeof(JEntity));

            var defaultIfEmptyCall = Expression.Call(genericDefaultIfEmptyMethodInfo, whereCall);

            //query => Queryable.Where(joinCollection, join => query.ForeignKeyId == (queryType)join.Id)
            var collectionSelectorExpression = CreateCollectionSelectorExpression<QEntity, JEntity>(queryParameter, defaultIfEmptyCall);

            //(queryCollection, joinCollection) =>
            //	Queryable.SelectMany(queryCollection, query =>
            //		Queryable.Where(joinCollection, join => query.ForeignKeyId == (queryType)join.Id),
            //	(i, j) => i);
            var expression = CreateExpression(joinCollectionParameter, collectionSelectorExpression, Bind(resultSelectorExpression));

            return expression.Compile()(query, table);
        }

        private static MemberInitExpression TestAuto(IEnumerable<string> path, Type type, ParameterExpression bindParam, Expression levelParam)
        {
            //Type: Test1 ()
            //levelParam: parameter root.
            var nextPropertyName = path.FirstOrDefault();
            var nextProperty = type.GetProperty(nextPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); //Test2 {}

            var assignments = new List<MemberAssignment>();

            //All empty:
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.Name != "EntityKey")
                .Where(p => p.Name != nextPropertyName)
                .Where(p => p.PropertyType.GetInterface(typeof(IEnumerable).FullName) == null && p.PropertyType.GetInterface(typeof(IEnumerable<>).FullName) == null);
            foreach (var prop in properties)
            {
                assignments.Add(
                    Expression.Bind(prop, Expression.Property(levelParam, prop.Name))
                );
            }

            if (path.Count() > 1)
            {
                var nextLevelParam = Expression.Property(levelParam, nextPropertyName);
                var nextType = nextProperty.PropertyType;
                var nextInit = TestAuto(path.Skip(1), nextType, bindParam, nextLevelParam);

                assignments.Add(
                    Expression.Bind(nextProperty, nextInit)
                );
            }
            else
            {
                assignments.Add(
                    Expression.Bind(nextProperty, bindParam)
                );
            }

            var newExpression = Expression.New(type);
            var init = Expression.MemberInit(newExpression, assignments);

            return init;
        }

        private static Expression<Func<TRoot, TBind, TRoot>> Bind<TRoot, TBind>(Expression<Func<TRoot, TBind>> func)
        {
            var path = new List<string>();
            Expression body = func.Body as MemberExpression;
            while (body != null)
            {
                var expression = (MemberExpression)body;
                path.Add(expression.Member.Name);
                body = expression.Expression as MemberExpression;
            }
            path.Reverse();

            var type1 = typeof(TRoot);
            var type2 = typeof(TBind);

            var parameter1 = Expression.Parameter(type1, "root");
            var parameter2 = Expression.Parameter(type2, "bind");
            var memberInit = TestAuto(path, type1, parameter2, parameter1);

            var lambda = Expression.Lambda<Func<TRoot, TBind, TRoot>>(memberInit, new[] { parameter1, parameter2 });
            //Console.WriteLine(lambda.ToString());
            return lambda;
        }

        private static Expression<Func<JEntity, bool>> CreateWherePredicate<QEntity, QKey, JEntity>(ParameterExpression queryParameter, Expression<Func<QEntity, QKey>> queryKey, string joinPropertyName)
        {
            var queryPropertyInfo = ((PropertyInfo)((MemberExpression)queryKey.Body).Member);

            var innerjoinCollectionParameter = Expression.Parameter(typeof(JEntity), "join");
            var innerRightProperty = Expression.Property(innerjoinCollectionParameter, joinPropertyName); //Street

            Expression bodyProperty = queryParameter;
            MemberExpression tempProperty = (MemberExpression)queryKey.Body;

            var listMemberNames = new List<string>();
            while (tempProperty != null)
            {
                listMemberNames.Add(tempProperty.Member.Name);
                tempProperty = tempProperty.Expression as MemberExpression;
            }

            foreach (var name in listMemberNames.AsEnumerable().Reverse())
                bodyProperty = Expression.Property(bodyProperty, name);

            Expression<Func<JEntity, bool>> wherePredicate = //j => i.Id == j.Id
                Expression.Lambda<Func<JEntity, bool>>(
                        Expression.Equal(bodyProperty, UnaryExpression.Convert(innerRightProperty, queryPropertyInfo.PropertyType)),
                        new[] { innerjoinCollectionParameter } //j =>
                );
            return wherePredicate;
        }

        private static MethodCallExpression CreateWhereCall<JEntity>(ParameterExpression joinCollectionParameter, Expression<Func<JEntity, bool>> wherePredicate)
        {
            var whereMethodInfo = GetWhereMethod();
            var genericWhereMethodInfo = whereMethodInfo.MakeGenericMethod(typeof(JEntity));

            var whereCall =
                Expression.Call(
                    genericWhereMethodInfo, //Queryable.Where<Test>
                    new Expression[] {
				joinCollectionParameter, //r.
				Expression.Quote(wherePredicate) //j => i.Id == j.Id
			});
            return whereCall;
        }

        private static Expression<Func<QEntity, IEnumerable<JEntity>>> CreateCollectionSelectorExpression<QEntity, JEntity>(ParameterExpression queryParameter, MethodCallExpression methodCall)
        {
            //Hvorfor IEnumerable?:
            Expression<Func<QEntity, IEnumerable<JEntity>>> collectionSelectorExpression = //i => Queryable.Where(r, j => j.Id == i.Id)
                Expression.Lambda<Func<QEntity, IEnumerable<JEntity>>>(
                            methodCall,
                            new[] { queryParameter } //i => 
                        );
            return collectionSelectorExpression;
        }

        private static Expression<Func<IQueryable<QEntity>, IQueryable<JEntity>, IQueryable<QEntity>>> CreateExpression<QEntity, JEntity>(
            ParameterExpression joinCollectionParameter,
            Expression<Func<QEntity, IEnumerable<JEntity>>> collectionSelectorExpression,
            Expression<Func<QEntity, JEntity, QEntity>> resultSelectorExpression
        )
        {
            var queryCollectionParameter = Expression.Parameter(typeof(IQueryable<QEntity>), "queryCollection");

            var selectManyMethodInfo = GetSelectManyMethod();
            var genericSelectManyMethodInfo = selectManyMethodInfo.MakeGenericMethod(typeof(QEntity), typeof(JEntity), typeof(QEntity));

            Expression<Func<IQueryable<QEntity>, IQueryable<JEntity>, IQueryable<QEntity>>> expression = //(l, r) => Queryable.SelectMany(l, i => Queryable.Where(r, j => j.Id == i.Id))
                Expression.Lambda<Func<IQueryable<QEntity>, IQueryable<JEntity>, IQueryable<QEntity>>>(
                        Expression.Call(
                            genericSelectManyMethodInfo, //Queryable.SelectMany<Test, Test, Test>
                            new Expression[] {
						queryCollectionParameter, //this IQueryable<QEntity> l
						Expression.Quote(collectionSelectorExpression), //i => r.Where(j => (j.Id == i.Id), //Her har vi "l" context.
						Expression.Quote(resultSelectorExpression) //(i, j) => i
					}),
                        new[] { queryCollectionParameter, joinCollectionParameter } //Parameters: (l, r) => SelectMany... Where...
                );
            return expression;
        }

        private static MethodInfo GetSelectManyMethod()
        {
            return typeof(Queryable).GetMethods()
                .Where(m => m.Name == "SelectMany")
                .Where(m => m.GetParameters().Count() == 3)
                .Single(m => m.GetParameters().Single(p => p.Name == "collectionSelector").ParameterType.GetGenericArguments().Single().GetGenericTypeDefinition() == typeof(Func<,>));
        }

        private static MethodInfo GetWhereMethod()
        {
            return typeof(Queryable).GetMethods()
                .Where(m => m.Name == "Where")
                .Single(m => m.GetParameters().Single(p => p.Name == "predicate").ParameterType.GetGenericArguments().Single().GetGenericTypeDefinition() == typeof(Func<,>));
        }

        private static MethodInfo GetDefaultIfEmpty()
        {
            return typeof(Queryable).GetMethods()
                .Where(m => m.Name == "DefaultIfEmpty")
                .Single(m => m.GetParameters().Count() == 1);
        }
    }
}
