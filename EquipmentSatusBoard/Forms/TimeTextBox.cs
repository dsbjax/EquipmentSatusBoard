using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EquipmentSatusBoard.Forms
{
    public class TimeTextBox : TextBox
    {

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch(Text.Length)
            {
                case 0:
                    if ((e.Key >= Key.D0 && e.Key <= Key.D2) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad2))
                        base.OnKeyDown(e);
                    else
                        e.Handled = true;

                    break;

                case 1:
                    if(Text == "2")
                    {
                        if ((e.Key >= Key.D0 && e.Key <= Key.D3) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad3))
                            base.OnKeyDown(e);
                        else
                            e.Handled = true;

                        break;
                    }
                    else
                    {
                        if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                            base.OnKeyDown(e);
                        else
                            e.Handled = true;

                        break;
                    }


                case 2:
                    if ((e.Key >= Key.D0 && e.Key <= Key.D5) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad5))
                        base.OnKeyDown(e);
                    else
                        e.Handled = true;

                    break;

                case 3:
                    if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                        base.OnKeyDown(e);
                    else
                        e.Handled = true;

                    break;

                case 4:
                    e.Handled = true;

                    TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                    UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                    if (keyboardFocus != null)
                    {
                        keyboardFocus.MoveFocus(tRequest);
                    }

                    break;

                default:
                    e.Handled = true;
                    break;
            }
        }
    }
}
