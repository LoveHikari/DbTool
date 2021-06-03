using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Win.Common
{
    public class FormHelper
    {
        /// <summary>
        /// 获得容器中的所有控件
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static List<Control> GetAllControls(Control control)
        {
            List<Control> controlList = new List<Control>();

            return GetAllControl(control, controlList);
        }

        /// <summary>
        /// 获得容器中的所有控件
        /// </summary>
        /// <param name="control"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static List<Control> GetAllControls(Control control, string type)
        {
            List<Control> controls = GetAllControls(control);
            return controls.Where(p => p.GetType().ToString() == type).ToList();
        }

        /// <summary>
        /// 获得容器中的所有控件
        /// </summary>
        /// <param name="control"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static List<Control> GetAllControls(Control control, Type type)
        {
            List<Control> controls = GetAllControls(control);
            return controls.Where(p => p.GetType() == type).ToList();
        }
        /// <summary>
        /// 获得容器中的所有控件
        /// </summary>
        /// <param name="control"></param>
        /// <param name="controlList"></param>
        /// <returns></returns>
        private static List<Control> GetAllControl(Control control, List<Control> controlList)
        {
            foreach (Control con in control.Controls)
            {
                if (con.Controls.Count > 0)
                {
                    GetAllControl(con, controlList);
                }
                controlList.Add(con);
            }
            return controlList;
        }
    }
}