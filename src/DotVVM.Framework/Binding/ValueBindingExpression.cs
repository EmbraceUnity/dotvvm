using System;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Parser;
using DotVVM.Framework.Parser.Translation;
using DotVVM.Framework.Utils;

namespace DotVVM.Framework.Binding
{
    /// <summary>
    /// A binding that gets the value from a viewmodel property.
    /// </summary>
    [BindingCompilationRequirements(Delegate = BindingCompilationRequirementType.StronglyRequire,
        OriginalString = BindingCompilationRequirementType.IfPossible,
        Javascript = BindingCompilationRequirementType.IfPossible,
        Expression = BindingCompilationRequirementType.IfPossible,
        UpdateDelegate = BindingCompilationRequirementType.IfPossible)]
    public class ValueBindingExpression : BindingExpression, IUpdatableBindingExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueBindingExpression"/> class.
        /// </summary>
        public ValueBindingExpression()
        {
        }

        public ValueBindingExpression(Func<object[], object> func, string javascript)
            : this((h, c) => func(h), javascript)
        { }

        public ValueBindingExpression(CompiledBindingExpression.BindingDelegate func, string javascript = null, CompiledBindingExpression.BindingUpdateDelegate updateFunc = null)
            : base(new CompiledBindingExpression()
            {
                Delegate = func,
                Javascript = javascript,
                UpdateDelegate = updateFunc
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueBindingExpression"/> class.
        /// </summary>
        public ValueBindingExpression(CompiledBindingExpression expression)
            : base(expression)
        {
        }

        /// <summary>
        /// Evaluates the binding.
        /// </summary>
        public override object Evaluate(DotvvmBindableControl control, DotvvmProperty property)
        {
            return ExecDelegate(control, property != DotvvmBindableControl.DataContextProperty);
        }

        /// <summary>
        /// Translates the binding to client script.
        /// </summary>
        public override string TranslateToClientScript(DotvvmBindableControl control, DotvvmProperty property)
        {
            return Javascript;
        }

        /// <summary>
        /// Updates the viewModel with the new value.
        /// </summary>
        public virtual void UpdateSource(object value, DotvvmBindableControl control, DotvvmProperty property)
        {
            ExecUpdateDelegate(control, value, property != DotvvmBindableControl.DataContextProperty);
        }
    }
}