﻿using AttendanceBussiness;
using LanguageResource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web.Mvc;

namespace DataGuard.Utilities
{
    /// <summary>
    /// 自定义注解属性
    /// </summary>
    public class SelectDisplayNameAttribute : Attribute
    {
        private string _diaplayName;
        public string DisplayName
        {
            get
            {
                return _diaplayName;
            }
        }
        public SelectDisplayNameAttribute(string displayName)
        {
            _diaplayName = displayName;
        }

    }

    public class EnumHelper
    {
        /// <summary>
        /// 获取自定义属性获取的内容
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Object obj)
        {

            //获取枚举对象的枚举类型
            Type type = obj.GetType();
            //通过反射获取该枚举类型的所有属性
            FieldInfo[] fieldInfos = type.GetFields();

            foreach (FieldInfo field in fieldInfos)
            {

                //不是参数obj,就直接跳过

                if (field.Name != obj.ToString())
                {
                    continue;
                }

                //取出参数obj的Display属性
                if (field.IsDefined(typeof(DisplayAttribute), true))
                {
                    return (field.GetCustomAttributes(typeof(DisplayAttribute), true)[0] as DisplayAttribute).GetDescription();
                }
                //取出参数obj的自定义属性
                if (field.IsDefined(typeof(SelectDisplayNameAttribute), true))
                {

                    return (field.GetCustomAttributes(typeof(SelectDisplayNameAttribute), true)[0] as SelectDisplayNameAttribute).DisplayName;
                }
                //取出参数 的EnumDisplayName属性
                if (field.IsDefined(typeof(EnumDisplayNameAttribute), true))
                {
                    string dip = (field.GetCustomAttributes(typeof(EnumDisplayNameAttribute), true)[0] as EnumDisplayNameAttribute).DisplayName;
                    dip = LangUtilities.GetStringReflectKeyName(dip);
                    return dip;
                }
            }
            return obj.ToString();
        }


        /// <summary>
        ///  将枚举类型的值和自定义属性配对组合为 List<SelectListItem>
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<SelectListItem> GetSelectList<T>()
        {
            var enumType = typeof(T);
            List<SelectListItem> selectList = new List<SelectListItem>();

            foreach (var obj in Enum.GetValues(enumType))
            {
                // 注意一定要Value = obj.ToString() 
                //原因是Value = (int)(obj.ToString()),这样取的枚举是对应的int值,在@Html.DropDownListFor中是无法显示默认选中值的,问题原因未解
                selectList.Add(new SelectListItem { Text = GetEnumDescription(obj), Value = obj.ToString() });
            }
            return selectList;
        }
    }
}