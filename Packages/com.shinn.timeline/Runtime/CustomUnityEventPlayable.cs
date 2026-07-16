using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Shinn.Timeline
{
    /// <summary>
    /// UnityEvent-style timeline clip: drag any Object (GameObject, Light,
    /// custom MonoBehaviour, ...) and pick a public method to invoke with a
    /// typed argument. A real UnityEvent cannot live on a clip because clips
    /// are project assets and cannot serialize scene references, so the clip
    /// stores the target via ExposedReference and the call via reflection.
    /// </summary>
    public class CustomUnityEventPlayable : PlayableAsset
    {
        public enum ParameterType
        {
            Void,
            Int,
            Float,
            String,
            Bool,
            Vector3,
            Vector2,
            Vector4,
            Color,
            Quaternion,
            ObjectReference,
            Enum,
        }

        public ExposedReference<Object> target;

        // Edited via CustomUnityEventPlayableEditor's method dropdown.
        [SerializeField] private string declaringTypeName = string.Empty;
        [SerializeField] private string methodName = string.Empty;
        [SerializeField] private ParameterType parameterType = ParameterType.Void;
        // Assembly-qualified parameter type, needed to disambiguate
        // ObjectReference and Enum parameters.
        [SerializeField] private string parameterTypeName = string.Empty;

        [SerializeField] private int intInput = 0;
        [SerializeField] private float floatInput = 0;
        [SerializeField] private string stringInput = string.Empty;
        [SerializeField] private bool boolInput = false;
        [SerializeField] private Vector3 vector3Input = Vector3.zero;
        [SerializeField] private Vector2 vector2Input = Vector2.zero;
        [SerializeField] private Vector4 vector4Input = Vector4.zero;
        [SerializeField] private Color colorInput = Color.white;
        [SerializeField] private Quaternion quaternionInput = Quaternion.identity;
        [SerializeField] private Object objectInput = null;
        [SerializeField] private int enumInput = 0;

        public string DeclaringTypeName => declaringTypeName;
        public string MethodName => methodName;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CustomUnityEventClip>.Create(graph);
            var clip = playable.GetBehaviour();

            clip.target = target.Resolve(graph.GetResolver());
            clip.declaringTypeName = declaringTypeName;
            clip.methodName = methodName;
            clip.parameterType = parameterType;
            clip.parameterTypeName = parameterTypeName;
            clip.intInput = intInput;
            clip.floatInput = floatInput;
            clip.stringInput = stringInput;
            clip.boolInput = boolInput;
            clip.vector3Input = vector3Input;
            clip.vector2Input = vector2Input;
            clip.vector4Input = vector4Input;
            clip.colorInput = colorInput;
            clip.quaternionInput = quaternionInput;
            clip.objectInput = objectInput;
            clip.enumInput = enumInput;

            return playable;
        }

        public class MethodCandidate
        {
            public string displayPath;
            public string declaringTypeName;
            public string methodName;
            public ParameterType parameterType;
            public string parameterTypeName;
        }

        // Base types whose methods are noise in the dropdown (SendMessage,
        // CancelInvoke, ...). Behaviour is kept so 'set_enabled' shows up.
        private static readonly Type[] skipDeclaringTypes =
        {
            typeof(Object), typeof(Component), typeof(MonoBehaviour),
        };

        /// <summary>
        /// Lists invocable methods on the dragged object. A GameObject offers
        /// its own methods plus every component's, like a UnityEvent dropdown.
        /// </summary>
        public static List<MethodCandidate> GetMethodCandidates(Object obj)
        {
            var list = new List<MethodCandidate>();
            if (obj == null)
                return list;

            if (obj is GameObject go)
            {
                AddCandidates(list, go);
                foreach (var component in go.GetComponents<Component>())
                {
                    if (component != null)
                        AddCandidates(list, component);
                }
            }
            else
            {
                AddCandidates(list, obj);
            }

            return list;
        }

        /// <summary>
        /// Maps the resolved ExposedReference back to the object the method
        /// lives on (e.g. a component when a GameObject was dragged).
        /// </summary>
        public static Object ResolveInvocationObject(Object resolvedTarget, string typeFullName)
        {
            if (resolvedTarget == null || string.IsNullOrEmpty(typeFullName))
                return null;

            if (resolvedTarget.GetType().FullName == typeFullName)
                return resolvedTarget;

            if (resolvedTarget is GameObject go)
            {
                foreach (var component in go.GetComponents<Component>())
                {
                    if (component != null && component.GetType().FullName == typeFullName)
                        return component;
                }
            }

            return null;
        }

        public static MethodInfo FindMethod(Object invokeObj, string _methodName, ParameterType _parameterType, string _parameterTypeName)
        {
            if (invokeObj == null || string.IsNullOrEmpty(_methodName))
                return null;

            foreach (var m in invokeObj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (m.Name != _methodName || m.ReturnType != typeof(void))
                    continue;
                if (!TryGetParameterType(m, out var pt, out var ptName))
                    continue;
                if (pt != _parameterType)
                    continue;
                // Disambiguate overloads sharing the same ParameterType
                // (e.g. two enum parameters).
                if (!string.IsNullOrEmpty(_parameterTypeName) && ptName != _parameterTypeName)
                    continue;
                return m;
            }

            return null;
        }

        private static void AddCandidates(List<MethodCandidate> list, Object obj)
        {
            var type = obj.GetType();
            foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (m.ReturnType != typeof(void))
                    continue;
                if (Array.IndexOf(skipDeclaringTypes, m.DeclaringType) >= 0)
                    continue;
                if (!TryGetParameterType(m, out var pt, out var ptName))
                    continue;

                list.Add(new MethodCandidate
                {
                    displayPath = type.Name + "/" + m.Name + " (" + GetParameterLabel(m, pt) + ")",
                    declaringTypeName = type.FullName,
                    methodName = m.Name,
                    parameterType = pt,
                    parameterTypeName = ptName,
                });
            }
        }

        private static string GetParameterLabel(MethodInfo m, ParameterType pt)
        {
            switch (pt)
            {
                case ParameterType.Void:
                    return "void";
                case ParameterType.ObjectReference:
                case ParameterType.Enum:
                    return m.GetParameters()[0].ParameterType.Name;
                default:
                    return pt.ToString().ToLower();
            }
        }

        private static bool TryGetParameterType(MethodInfo m, out ParameterType pt, out string ptName)
        {
            pt = ParameterType.Void;
            ptName = string.Empty;

            var ps = m.GetParameters();
            if (ps.Length == 0)
                return true;
            if (ps.Length != 1)
                return false;

            var t = ps[0].ParameterType;

            if (t == typeof(int)) { pt = ParameterType.Int; return true; }
            if (t == typeof(float)) { pt = ParameterType.Float; return true; }
            if (t == typeof(string)) { pt = ParameterType.String; return true; }
            if (t == typeof(bool)) { pt = ParameterType.Bool; return true; }
            if (t == typeof(Vector3)) { pt = ParameterType.Vector3; return true; }
            if (t == typeof(Vector2)) { pt = ParameterType.Vector2; return true; }
            if (t == typeof(Vector4)) { pt = ParameterType.Vector4; return true; }
            if (t == typeof(Color)) { pt = ParameterType.Color; return true; }
            if (t == typeof(Quaternion)) { pt = ParameterType.Quaternion; return true; }

            // Only Enum/ObjectReference need the concrete type recorded, to
            // disambiguate overloads and drive the inspector field.
            if (t.IsEnum)
            {
                pt = ParameterType.Enum;
                ptName = t.AssemblyQualifiedName;
                return true;
            }
            if (typeof(Object).IsAssignableFrom(t))
            {
                pt = ParameterType.ObjectReference;
                ptName = t.AssemblyQualifiedName;
                return true;
            }

            return false;
        }
    }
}
