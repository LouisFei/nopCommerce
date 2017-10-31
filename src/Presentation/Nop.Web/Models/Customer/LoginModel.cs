using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using FluentValidation.Attributes;

using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Customer;

namespace Nop.Web.Models.Customer
{
    /// <summary>
    /// 登录视图模型（含验证规则）
    /// </summary>
    [Validator(typeof(LoginValidator))]
    public partial class LoginModel : BaseNopModel
    {
        /// <summary>
        /// 实例化用户登录视图模型
        /// </summary>
        public LoginModel() { }

        public bool CheckoutAsGuest { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [NopResourceDisplayName("Account.Login.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        /// <summary>
        /// 是否使用用户名来代替电子邮箱登录
        /// </summary>
        public bool UsernamesEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NopResourceDisplayName("Account.Login.Fields.UserName")]
        [AllowHtml]
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DataType(DataType.Password)]
        [NoTrim]
        [NopResourceDisplayName("Account.Login.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        /// <summary>
        /// 记住我
        /// </summary>
        [NopResourceDisplayName("Account.Login.Fields.RememberMe")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// 是否显示验证码
        /// </summary>
        public bool DisplayCaptcha { get; set; }
    }
}