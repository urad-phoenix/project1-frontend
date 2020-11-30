namespace Phoenix.Playables.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Linq;

    public static class ReflectionUtility
    {       
        private static Assembly[] loadedAssemblies;

        private static readonly Dictionary<string, Type> typeLookup = new Dictionary<string, Type>();

        private static List<Type> types = new List<Type>();

        public static T[] GetCustomAttributes<T>(bool inherited) where T : System.Attribute
        {
            if (ReflectionUtility.loadedAssemblies == null)
            {
                ReflectionUtility.loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            }
            List<T> result = new List<T>();
            for (int j = 0; j < ReflectionUtility.loadedAssemblies.Length; j++)
            {
                result.AddRange(ReflectionUtility.loadedAssemblies[j].GetTypes().
                    Where(x => x.IsDefined(typeof(T))).
                    Select(x => x.GetCustomAttribute<T>()));                
            }
            return result.ToArray();
        }

        public static T[] GetCustomAttributes<T>(Type type, bool inherited) where T : System.Attribute
        {           
#if NETFX_CORE
			    return (T[]) type.GetTypeInfo().GetCustomAttributes(typeof(T), inherited);
#else
            return (T[])type.GetCustomAttributes(typeof(T), inherited);
#endif
        }

        public static Assembly[] GetLoadedAssemblies()
        {
            if (loadedAssemblies == null)
            {
                loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            }
            return loadedAssemblies;
        }

        [Localizable(false)]
        public static Type GetGlobalType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            Type type;
            ReflectionUtility.typeLookup.TryGetValue(typeName, out type);
            if (type != null)
            {
                return type;
            }

            type = Type.GetType(typeName);

            if (type == null)
            {
                if (ReflectionUtility.loadedAssemblies == null)
                {
                    ReflectionUtility.loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();                   
                }                

                if (type == null)
                {
                    for (int j = 0; j < ReflectionUtility.loadedAssemblies.Length; j++)
                    {
                        Type[] types = ReflectionUtility.loadedAssemblies[j].GetTypes();
                        for (int k = 0; k < types.Length; k++)
                        {
                            if (types[k].Name == typeName)
                            {
                                type = types[k];
                                ReflectionUtility.typeLookup[typeName] = type;
                                return type;
                            }
                        }
                    }
                }
            }
            ReflectionUtility.typeLookup.Remove(typeName);
            ReflectionUtility.typeLookup[typeName] = type;
            return type;
        }

        public static Type GetPropertyType(Type type, string path)
        {
            string[] array = path.Split(new char[]
            {
            '.'
            });
            for (int i = 0; i < array.Length; i++)
            {
                string name = array[i];
                PropertyInfo property = type.GetProperty(name);
                if (property != null)
                {
                    type = property.PropertyType;
                }
                else
                {
                    FieldInfo field = type.GetField(name);
                    if (field == null)
                    {
                        return null;
                    }
                    type = field.FieldType;
                }
            }
            return type;
        }

        public static MemberInfo[] GetMemberInfo(Type type, string path)
        {
            if (type == null)
            {
                return null;
            }
            string[] array = path.Split(new char[]
            {
            '.'
            });
            MemberInfo[] array2 = new MemberInfo[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                string name = array[i];
                PropertyInfo property = type.GetProperty(name);
                if (property != null)
                {
                    array2[i] = property;
                    type = property.PropertyType;
                }
                else
                {
                    FieldInfo field = type.GetField(name);
                    if (field == null)
                    {
                        return null;
                    }
                    array2[i] = field;
                    type = field.FieldType;
                }
            }
            return array2;
        }

        public static bool CanReadMemberValue(MemberInfo member)
        {
            MemberTypes memberType = member.MemberType;
            return memberType == MemberTypes.Field || (memberType == MemberTypes.Property && ((PropertyInfo)member).CanRead);
        }

        public static bool CanSetMemberValue(MemberInfo member)
        {
            MemberTypes memberType = member.MemberType;
            return memberType == MemberTypes.Field || (memberType == MemberTypes.Property && ((PropertyInfo)member).CanWrite);
        }

        public static bool CanGetMemberValue(MemberInfo member)
        {
            MemberTypes memberType = member.MemberType;
            return memberType == MemberTypes.Field || (memberType == MemberTypes.Property && ((PropertyInfo)member).CanRead);
        }

        public static Type GetMemberUnderlyingType(MemberInfo member)
        {
            MemberTypes memberType = member.MemberType;
            if (memberType == MemberTypes.Event)
            {
                return ((EventInfo)member).EventHandlerType;
            }
            if (memberType == MemberTypes.Field)
            {
                return ((FieldInfo)member).FieldType;
            }
            if (memberType != MemberTypes.Property)
            {
                throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or EventInfo", "member");
            }
            return ((PropertyInfo)member).PropertyType;
        }

        public static object GetMemberValue(MemberInfo[] memberInfo, object target)
        {
            for (int i = 0; i < memberInfo.Length; i++)
            {
                target = ReflectionUtility.GetMemberValue(memberInfo[i], target);
            }
            return target;
        }

        public static object GetMemberValue(MemberInfo member, object target)
        {
            MemberTypes memberType = member.MemberType;
            if (memberType != MemberTypes.Field)
            {
                if (memberType == MemberTypes.Property)
                {
                    try
                    {
                        return ((PropertyInfo)member).GetValue(target, null);
                    }
                    catch (TargetParameterCountException innerException)
                    {
                        throw new ArgumentException("MemberInfo has index parameters", "member", innerException);
                    }
                }
                throw new ArgumentException("MemberInfo is not of type FieldInfo or PropertyInfo", "member");
            }
            return ((FieldInfo)member).GetValue(target);
        }

        public static void SetMemberValue(MemberInfo member, object target, object value)
        {
            MemberTypes memberType = member.MemberType;
            if (memberType == MemberTypes.Field)
            {
                ((FieldInfo)member).SetValue(target, value);
                return;
            }
            if (memberType != MemberTypes.Property)
            {
                throw new ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", "member");
            }
            ((PropertyInfo)member).SetValue(target, value, null);
        }

        public static void SetMemberValue(MemberInfo[] memberInfo, object target, object value)
        {
            object parent = null;
            MemberInfo targetInfo = null;
            for (int i = 0; i < memberInfo.Length - 1; i++)
            {
                parent = target;
                targetInfo = memberInfo[i];
                target = ReflectionUtility.GetMemberValue(memberInfo[i], target);
            }
            if (target.GetType().IsValueType)
            {
                ReflectionUtility.SetBoxedMemberValue(parent, targetInfo, target, memberInfo[memberInfo.Length - 1], value);
                return;
            }
            ReflectionUtility.SetMemberValue(memberInfo[memberInfo.Length - 1], target, value);
        }

        public static void SetBoxedMemberValue(object parent, MemberInfo targetInfo, object target, MemberInfo propertyInfo, object value)
        {
            ReflectionUtility.SetMemberValue(propertyInfo, target, value);
            ReflectionUtility.SetMemberValue(targetInfo, parent, target);
        }

        public static List<MemberInfo> GetFieldsAndProperties<T>(BindingFlags bindingAttr)
        {
            return ReflectionUtility.GetFieldsAndProperties(typeof(T), bindingAttr);
        }

        public static List<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {
            List<MemberInfo> expr_05 = new List<MemberInfo>();
            expr_05.AddRange(type.GetFields(bindingAttr));
            expr_05.AddRange(type.GetProperties(bindingAttr));
            return expr_05;
        }

        public static FieldInfo[] GetPublicFields(this Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        public static T GetAttribute<T>(IEnumerable<object> attributes) where T : Attribute
        {
            if(attributes == null)
            {
                T result = default(T);
                return result;
            }

            using (IEnumerator<object> enumerator = attributes.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    T t = ((Attribute)enumerator.Current) as T;

                    if (t != null)
                    {
                        T result = t;
                        return result;
                    }
                }
            }

            return default(T);
        }       
    }
}
