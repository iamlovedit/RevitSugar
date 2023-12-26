using Autodesk.Revit.DB;
using System;

namespace RevitSugar.DB
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool UpdateParameterValue(string parameterName, dynamic value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool UpdateParameterValue(Guid guid, dynamic value);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builtInParameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool UpdateParameterValue(BuiltInParameter builtInParameter, dynamic value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="definition"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool UpdateParameterValue(Definition definition, dynamic value);
    }

    public class Updateable : IUpdateable
    {
        private readonly Element _element;

        public Updateable(Element element)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
        }

        /// <inheritdoc/>
        public bool UpdateParameterValue(string parameterName, dynamic value)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentException($"'{nameof(parameterName)}' cannot be null or empty.", nameof(parameterName));
            }

            if (value is ElementId || value is int || value is string || value is double)
            {
                var parameter = _element.LookupParameter(parameterName);
                return SetValue(parameter, value);
            }
            throw new ArgumentException($"{value} is invalid parameter value");
        }

        /// <inheritdoc/>
        public bool UpdateParameterValue(Guid guid, dynamic value)
        {
            if (value is ElementId || value is int || value is string || value is double)
            {
                var parameter = _element.get_Parameter(guid);
                return SetValue(parameter, value);
            }
            throw new ArgumentException($"{value} is invalid parameter value");
        }

        /// <inheritdoc/>
        public bool UpdateParameterValue(BuiltInParameter builtInParameter, dynamic value)
        {
            if (value is ElementId || value is int || value is string || value is double)
            {
                var parameter = _element.get_Parameter(builtInParameter);
                return SetValue(parameter, value);
            }
            throw new ArgumentException($"{value} is invalid parameter value");
        }

        /// <inheritdoc/>
        public bool UpdateParameterValue(Definition definition, dynamic value)
        {
            if (definition is null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            var parameter = _element.get_Parameter(definition);
            return SetValue(parameter, value);
        }

        private bool SetValue(Parameter parameter, dynamic value)
        {
            return parameter is null ? throw new Exception("parameter is not found") : parameter.Set(value);
        }
    }
}
