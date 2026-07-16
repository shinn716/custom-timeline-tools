using System;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Shinn.Timeline
{
    public class CustomUnityEventClip : PlayableBehaviour
    {
        public Object target { get; set; }
        public string declaringTypeName { get; set; }
        public string methodName { get; set; }
        public CustomUnityEventPlayable.ParameterType parameterType { get; set; }
        public string parameterTypeName { get; set; }

        public int intInput { get; set; }
        public float floatInput { get; set; }
        public string stringInput { get; set; }
        public bool boolInput { get; set; }
        public Vector3 vector3Input { get; set; }
        public Vector2 vector2Input { get; set; }
        public Vector4 vector4Input { get; set; }
        public Color colorInput { get; set; }
        public Quaternion quaternionInput { get; set; }
        public Object objectInput { get; set; }
        public int enumInput { get; set; }

        private bool fired = false;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // Never invoke user methods while scrubbing in edit mode.
            if (!Application.isPlaying)
                return;

            if (fired)
                return;

            fired = true;
            Invoke();
        }

        private void Invoke()
        {
            if (target == null || string.IsNullOrEmpty(methodName))
                return;

            var invokeObj = CustomUnityEventPlayable.ResolveInvocationObject(target, declaringTypeName);
            if (invokeObj == null)
            {
                Debug.LogWarning("CustomUnityEventClip: type '" + declaringTypeName + "' not found on target '" + target.name + "'.", target);
                return;
            }

            var method = CustomUnityEventPlayable.FindMethod(invokeObj, methodName, parameterType, parameterTypeName);
            if (method == null)
            {
                Debug.LogWarning("CustomUnityEventClip: method '" + methodName + "' not found on '" + declaringTypeName + "'.", target);
                return;
            }

            try
            {
                if (parameterType == CustomUnityEventPlayable.ParameterType.Void)
                    method.Invoke(invokeObj, null);
                else
                    method.Invoke(invokeObj, new object[] { GetArgument() });
            }
            catch (Exception e)
            {
                Debug.LogWarning("CustomUnityEventClip: failed to invoke '" + methodName + "'\n" + e, target);
            }
        }

        private object GetArgument()
        {
            switch (parameterType)
            {
                case CustomUnityEventPlayable.ParameterType.Int:
                    return intInput;
                case CustomUnityEventPlayable.ParameterType.Float:
                    return floatInput;
                case CustomUnityEventPlayable.ParameterType.String:
                    return stringInput;
                case CustomUnityEventPlayable.ParameterType.Bool:
                    return boolInput;
                case CustomUnityEventPlayable.ParameterType.Vector3:
                    return vector3Input;
                case CustomUnityEventPlayable.ParameterType.Vector2:
                    return vector2Input;
                case CustomUnityEventPlayable.ParameterType.Vector4:
                    return vector4Input;
                case CustomUnityEventPlayable.ParameterType.Color:
                    return colorInput;
                case CustomUnityEventPlayable.ParameterType.Quaternion:
                    return quaternionInput;
                case CustomUnityEventPlayable.ParameterType.ObjectReference:
                    return objectInput;
                case CustomUnityEventPlayable.ParameterType.Enum:
                    var enumType = Type.GetType(parameterTypeName);
                    return enumType != null && enumType.IsEnum
                        ? System.Enum.ToObject(enumType, enumInput)
                        : (object)enumInput;
                default:
                    return null;
            }
        }
    }
}
