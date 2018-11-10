using System;
using System.Linq;
using System.Linq.Expressions;

namespace EnumParser
{
    public class EnumParser
    {
        private static object ParseWithMapping(Type type, string name)
        {
            var attr = type.GetCustomAttributes(false).OfType<EnumNameAttribute>().FirstOrDefault();

            if (attr != null)
            {
                var field = type.GetFields()
                    .Select(f => new { FieldName = f.Name, Ca = f.GetCustomAttributes(typeof(EnumNameAttribute), false).OfType<EnumNameAttribute>().FirstOrDefault()?.Value })
                    .FirstOrDefault(x => x.Ca == name);

                if (field == null)
                {
                    return null;
                }

                return Enum.Parse(type, field.FieldName);
            }

            return null;
        }

        public static object Parse(Type type, string name)
        {
            var splits = name.Split(';');

            var retVal = splits.Select(x =>
            {
                var obj = ParseWithMapping(type, x);

                if (obj == null)
                {
                    obj = Enum.Parse(type, x);
                }

                return obj;
            }).Aggregate((x, y) => EnumOr(x, y));

            return retVal;
        }

        public static object EnumOr(object enum1, object enum2)
        {
            var t1 = enum1.GetType();
            var t2 = enum2.GetType();

            if (t1 != t2)
            {
                throw new Exception($"both enum types are not equal: {enum1}, {enum2}");
            }

            var enumsType = enum1.GetType().GetEnumUnderlyingType();

            // cast object to int/long
            var p1 = Expression.Parameter(typeof(object), "e1");
            var convExpr1 = Expression.Convert(p1, enumsType);
            var p2 = Expression.Parameter(typeof(object), "e2");
            var convExpr2 = Expression.Convert(p2, enumsType);
            var or = Expression.Or(convExpr1, convExpr2);
            // cast int to enum type
            var toEnumConv = Expression.Convert(or, t1);

            // we now must call Expression.Lambda<Func<object, object, EnumType>>(...)
            // to do this, build method call through reflection

            var genericLambda = typeof(Expression).GetMethods().First(x => x.Name.Contains("Lambda") && x.IsGenericMethod);
            var genericFuncType = typeof(Func<,,>);
            var parameterizedFuncType = genericFuncType.MakeGenericType(typeof(object), typeof(object), t1);
            var lambdaWithParameter = genericLambda.MakeGenericMethod(parameterizedFuncType);
            var lambdaResult = lambdaWithParameter.Invoke(null, new object[] { toEnumConv, new ParameterExpression[] { p1, p2 } });

            // now call compile()
            var compileMethod = lambdaResult.GetType().GetMethods().First(x => x.Name == "Compile" && x.GetParameters().Count() == 0);
            var compileResult = compileMethod.Invoke(lambdaResult, new object[] { });

            // now call compiled function
            var retVal = compileResult.GetType().GetMethod("Invoke").Invoke(compileResult, new object[] { enum1, enum2 });

            return retVal;
        }
    }
}
