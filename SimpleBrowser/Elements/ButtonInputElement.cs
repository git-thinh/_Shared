// -----------------------------------------------------------------------
// <copyright file="ButtonInputElement.cs" company="SimpleBrowser">
// See https://github.com/axefrog/SimpleBrowser/blob/master/readme.md
// </copyright>
// -----------------------------------------------------------------------

namespace SimpleBrowser.Elements
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>
    /// Implements an HTML button or input submit element
    /// </summary>
    internal class ButtonInputElement : InputElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonInputElement"/> class.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> associated with this element.</param>
        public ButtonInputElement(XElement element)
            : base(element)
        {
        }

        /// <summary>
        /// Gets the form values to submit for this input
        /// </summary>
        /// <param name="isClickedElement">True, if the action to submit the form was clicking this element. Otherwise, false.</param>
        /// <returns>A collection of <see cref="UserVariableEntry"/> objects.</returns>
        public override IEnumerable<UserVariableEntry> ValuesToSubmit(bool isClickedElement)
        {
            if (isClickedElement && !string.IsNullOrEmpty(this.Name))
            {
                return base.ValuesToSubmit(isClickedElement);
            }

            return new UserVariableEntry[0];
        }

        /// <summary>
        /// Perform a click action on the label element.
        /// </summary>
        /// <returns>The <see cref="ClickResult"/> of the operation.</returns>
        public override ClickResult Click()
        {
            if (this.Disabled)
            {
                return ClickResult.SucceededNoOp;
            }

            base.Click();
            if (this.SubmitForm(clickedElement: this))
            {
                return ClickResult.SucceededNavigationComplete;
            }

            return ClickResult.SucceededNavigationError;
        }
    }
}
