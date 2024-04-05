namespace Cs.Dynamic
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading.Tasks;

    public static class DelegateFactory<TDelegate> where TDelegate : class
    {
        private static readonly MethodInfo ResultMethod;
        private static readonly ParameterInfo[] ResultParameters;
        private static readonly MethodInfo CompletedTaskCaller;

        static DelegateFactory()
        {
            ResultMethod = typeof(TDelegate).GetMethod("Invoke");
            ResultParameters = ResultMethod.GetParameters();

            var taskType = typeof(Task);
            if (taskType.IsAssignableFrom(ResultMethod.ReturnType))
            {
                if (ResultMethod.ReturnType == taskType)
                {
                    var propertyInfo = taskType.GetProperty(nameof(Task.CompletedTask), BindingFlags.Static | BindingFlags.Public);
                    CompletedTaskCaller = propertyInfo.GetGetMethod();
                }
                else
                {
                    var methodInfo = taskType.GetMethod(nameof(Task.FromResult), BindingFlags.Static | BindingFlags.Public);
                    CompletedTaskCaller = methodInfo.MakeGenericMethod(ResultMethod.ReturnType.GetGenericArguments());
                }
            }
        }

        public static TDelegate CreateAction(MethodInfo methodInfo)
        {
            if (ResultMethod.ReturnType != typeof(void))
            {
                throw new ArgumentException($"CreateAction need void return delegate");
            }

            if (!IsConvertableAction(methodInfo))
            {
                throw new ArgumentException($"Method signature mismatched.");
            }

            var dynamicMethod = new DynamicMethod(
                name: string.Empty,
                ResultMethod.ReturnType,
                ResultParameters.Select(e => e.ParameterType).ToArray(),
                methodInfo.DeclaringType.Module);
            var il = dynamicMethod.GetILGenerator();
            for (int i = 0; i < ResultParameters.Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i);
            }

            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }

        public static TDelegate CreateMemberFunction(MethodInfo methodInfo)
        {
            if (ResultMethod.ReturnType == typeof(void))
            {
                throw new ArgumentException($"CreateMemberFunction delegate has no return type.");
            }

            if (!IsConvertableMemberFunction(methodInfo))
            {
                throw new ArgumentException($"Method signature mismatched.");
            }

            var dynamicMethod = new DynamicMethod(
                name: string.Empty,
                ResultMethod.ReturnType,
                ResultParameters.Select(e => e.ParameterType).ToArray(),
                methodInfo.DeclaringType.Module);
            var il = dynamicMethod.GetILGenerator();
            for (int i = 0; i < ResultParameters.Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i);
            }

            il.Emit(OpCodes.Call, methodInfo);
            il.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }

        public static TDelegate CreateAwaitable(MethodInfo methodInfo)
        {
            if (ResultMethod.ReturnType != typeof(Task))
            {
                throw new ArgumentException($"CreateAwaitableFunction need Task return delegate");
            }

            if (!IsConvertableFunction(methodInfo))
            {
                throw new ArgumentException($"Method signature mismatched.");
            }

            var dynamicMethod = new DynamicMethod(
                name: string.Empty,
                ResultMethod.ReturnType,
                ResultParameters.Select(e => e.ParameterType).ToArray(),
                methodInfo.DeclaringType.Module);
            var il = dynamicMethod.GetILGenerator();
            for (int i = 0; i < ResultParameters.Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i);
            }

            il.Emit(OpCodes.Call, methodInfo);
            if (methodInfo.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Call, CompletedTaskCaller);
            }
            else
            {
                il.Emit(OpCodes.Castclass, ResultMethod.ReturnType);
            }

            il.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }

        public static TDelegate CreateAwaitableMemberFunction(MethodInfo methodInfo)
        {
            var returnType = methodInfo.ReturnType;
            if (returnType == typeof(void))
            {
                throw new ArgumentException($"CreateAwaitableMemberFunction delegate invalid return type:{ResultMethod.ReturnType}");
            }

            if (!IsConvertableAwaitableMemberFunction(methodInfo))
            {
                throw new ArgumentException($"Method signature mismatched.");
            }

            var dynamicMethod = new DynamicMethod(
                name: string.Empty,
                ResultMethod.ReturnType,
                ResultParameters.Select(e => e.ParameterType).ToArray(),
                methodInfo.DeclaringType.Module);
            var il = dynamicMethod.GetILGenerator();
            for (int i = 0; i < ResultParameters.Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i);
            }

            il.Emit(OpCodes.Call, methodInfo);
            if (returnType.IsGenericType == false)
            {
                il.Emit(OpCodes.Call, CompletedTaskCaller);
            }

            il.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }

        public static TDelegate CreateAwaitableMemberAction(MethodInfo methodInfo)
        {
            var returnType = ResultMethod.ReturnType;
            if (returnType != typeof(void) && returnType != typeof(Task))
            {
                throw new ArgumentException($"CreateAwaitableMemberAction delegate invalid return type:{ResultMethod.ReturnType}");
            }

            if (!IsConvertableAwaitableMemberAction(methodInfo))
            {
                throw new ArgumentException($"Method signature mismatched.");
            }

            var dynamicMethod = new DynamicMethod(
                name: string.Empty,
                ResultMethod.ReturnType,
                ResultParameters.Select(e => e.ParameterType).ToArray(),
                methodInfo.DeclaringType.Module);
            var il = dynamicMethod.GetILGenerator();
            for (int i = 0; i < ResultParameters.Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i);
            }

            il.Emit(OpCodes.Call, methodInfo);
            if (methodInfo.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Call, CompletedTaskCaller);
            }
            else
            {
                il.Emit(OpCodes.Castclass, ResultMethod.ReturnType);
            }

            il.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
        }

        //// ------------------------------------------------------------------------------------------------------------

        private static bool IsConvertableAction(MethodInfo target)
        {
            var targetParameters = target.GetParameters();

            if (ResultMethod.ReturnType != target.ReturnType)
            {
                return false;
            }

            if (ResultParameters.Length != targetParameters.Length)
            {
                return false;
            }

            for (int i = 0; i < ResultParameters.Length; i++)
            {
                if (ResultParameters[i].ParameterType.IsAssignableFrom(targetParameters[i].ParameterType) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsConvertableFunction(MethodInfo target)
        {
            var targetParameters = target.GetParameters();

            if (ResultMethod.ReturnType != target.ReturnType &&
                target.ReturnType != typeof(void))
            {
                return false;
            }

            if (ResultParameters.Length != targetParameters.Length)
            {
                return false;
            }

            for (int i = 0; i < ResultParameters.Length; i++)
            {
                if (ResultParameters[i].ParameterType.IsAssignableFrom(targetParameters[i].ParameterType) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsConvertableMemberFunction(MethodInfo target)
        {
            var targetParameters = target.GetParameters();

            if (ResultMethod.ReturnType != target.ReturnType &&
                target.ReturnType != typeof(void))
            {
                return false;
            }

            if (ResultParameters.Length != targetParameters.Length + 1)
            {
                return false;
            }

            for (int i = 0; i < targetParameters.Length; i++)
            {
                if (ResultParameters[i + 1].ParameterType.IsAssignableFrom(targetParameters[i].ParameterType) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsConvertableAwaitableMemberFunction(MethodInfo target)
        {
            var targetParameters = target.GetParameters();

            if (ResultMethod.ReturnType != target.ReturnType && ResultMethod.ReturnType.GenericTypeArguments[0] != target.ReturnType)
            {
                return false;
            }

            if (ResultParameters.Length != targetParameters.Length + 1)
            {
                return false;
            }

            for (int i = 0; i < targetParameters.Length; i++)
            {
                if (ResultParameters[i + 1].ParameterType.IsAssignableFrom(targetParameters[i].ParameterType) == false)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsConvertableAwaitableMemberAction(MethodInfo target)
        {
            var targetParameters = target.GetParameters();

            if (ResultMethod.ReturnType != target.ReturnType && target.ReturnType != typeof(void))
            {
                return false;
            }

            if (ResultParameters.Length != targetParameters.Length + 1)
            {
                return false;
            }

            for (int i = 0; i < targetParameters.Length; i++)
            {
                if (ResultParameters[i + 1].ParameterType.IsAssignableFrom(targetParameters[i].ParameterType) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
