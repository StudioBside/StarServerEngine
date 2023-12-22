namespace Cs.Dynamic
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    public sealed class PropertyAccess<TOwner, TProperty>
    {
        private readonly PropertyInfo propertyInfo;
        private readonly Func<TOwner, TProperty> getDelegate;
        private readonly Action<TOwner, TProperty> setDelegate;

        public PropertyAccess(string propertyName) : this(typeof(TOwner).GetProperty(propertyName))
        {
        }

        public PropertyAccess(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
            var propertyType = this.propertyInfo.PropertyType;
            if (typeof(TProperty).IsAssignableFrom(propertyType) == false)
            {
                throw new Exception($"invalid propertyType. expected:{typeof(TProperty).Name} actual:{propertyType.Name}");
            }

            if (propertyType.IsValueType)
            {
                throw new Exception($"propertyType({propertyType.Name}) should be reference type. valueType not supported.");
            }

            if (this.propertyInfo.CanRead)
            {
                var dynamicGet = new DynamicMethod(
                String.Empty,
                typeof(TProperty),
                new Type[] { typeof(TOwner) },
                this.propertyInfo.DeclaringType,
                skipVisibility: true);

                var il = dynamicGet.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, this.propertyInfo.GetMethod);
                il.Emit(OpCodes.Ret);

                this.getDelegate = (Func<TOwner, TProperty>)dynamicGet.CreateDelegate(typeof(Func<TOwner, TProperty>));
            }

            var setMethodInfo = this.propertyInfo.GetSetMethod();
            if (setMethodInfo != null)
            {
                var dynamicSet = new DynamicMethod(
                    String.Empty,
                    typeof(void),
                    new Type[] { typeof(TOwner), typeof(TProperty) },
                    this.propertyInfo.DeclaringType,
                    skipVisibility: true);

                var il = dynamicSet.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, setMethodInfo);
                il.Emit(OpCodes.Ret);

                this.setDelegate = (Action<TOwner, TProperty>)dynamicSet.CreateDelegate(typeof(Action<TOwner, TProperty>));
            }
        }

        public string Name => this.propertyInfo.Name;

        public TProperty GetValue(TOwner owner)
        {
            return this.getDelegate(owner);
        }

        public void SetValue(TOwner owner, TProperty property)
        {
            this.setDelegate(owner, property);
        }
    }
}
