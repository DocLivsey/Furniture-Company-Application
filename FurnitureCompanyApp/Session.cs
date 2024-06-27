using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace FurnitureCompanyApp
{
    public class Session
    {
        public static class FormsAction
        {
            
            public enum ValidationMode
            {
                NameValidation, IdValidation, PriceValidation
            }
            
            public static bool ValidateBoxInput(Control box, ValidationMode mode, string toolTipText)
            {
                ToolTip toolTip = GetErrorToolTip();
                switch (mode)
                {
                    case ValidationMode.IdValidation:
                        if (!box.Text.All(char.IsDigit) || box.Text.Trim().Length == 0)
                        {
                            
                                box.BackColor = Color.Red;
                                toolTip.SetToolTip(box, toolTipText);
                                return false;
                            
                        }
                        box.BackColor = Color.White;
                        toolTip.Active = false;
                        return true;
                    
                    case ValidationMode.NameValidation:
                        if (!box.Text.Trim().Equals(box.Text) || box.Text.Trim().Length == 0)
                        {
                            box.BackColor = Color.Red;
                            toolTip.SetToolTip(box, toolTipText);
                            return false;
                        }
                        box.BackColor = Color.White;
                        toolTip.Active = false;
                        return true;
                    
                    case ValidationMode.PriceValidation:
                        if (!Regex.IsMatch(box.Text, @"^[0-9]+(?:[,]\d*)?\z") || box.Text.Trim().Length == 0)
                        {
                            box.BackColor = Color.Red;
                            toolTip.SetToolTip(box, toolTipText);
                            return false;
                        } 
                        box.BackColor = Color.White; 
                        toolTip.Active = false;
                        return true;
                    
                    default:
                        throw new Exception("The Validation mode cant be NULL\n" +
                                            "Please select correct validation mode");
                }
            }

            private static ToolTip GetErrorToolTip()
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Active = true;
                toolTip.AutoPopDelay = 4000;
                toolTip.InitialDelay = 600;
                toolTip.IsBalloon = true;
                toolTip.ToolTipIcon = ToolTipIcon.Error;
                return toolTip;
            }

            public static void SetErrorToolTip(Control box, string text)
            {
                ToolTip toolTip = GetErrorToolTip();
                toolTip.SetToolTip(box, text);
            }
        }
    }
}