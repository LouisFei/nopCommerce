using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nop.Core.Configuration;
using Nop.Core.Domain.Configuration;

namespace Nop.Services.Configuration
{
    /// <summary>
    /// Setting service interface
    /// 设置服务接口
    /// </summary>
    public partial interface ISettingService
    {
        /// <summary>
        /// Gets a setting by identifier
        /// 获得一个设置
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        Setting GetSettingById(int settingId);

        /// <summary>
        /// Deletes a setting
        /// 删除一个设置
        /// </summary>
        /// <param name="setting">Setting</param>
        void DeleteSetting(Setting setting);

        /// <summary>
        /// Deletes settings
        /// 批量删除设置
        /// </summary>
        /// <param name="settings">Settings</param>
        void DeleteSettings(IList<Setting> settings);

        /// <summary>
        /// Get setting by key
        /// 获得设置（根据键值）
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting</returns>
        Setting GetSetting(string key, int storeId = 0, bool loadSharedValueIfNotFound = false);

        /// <summary>
        /// Get setting value by key
        /// 获得设置值（根据键值）
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>Setting value</returns>
        T GetSettingByKey<T>(string key, T defaultValue = default(T), 
            int storeId = 0, bool loadSharedValueIfNotFound = false);
        
        /// <summary>
        /// Set setting value
        /// 设置设置值
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true);

        /// <summary>
        /// Gets all settings
        /// 获得所有设置
        /// </summary>
        /// <returns>Settings</returns>
        IList<Setting> GetAllSettings();

        /// <summary>
        /// Determines whether a setting exists
        /// 判断一个设置是否存在（支持表达式语法）
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>true -setting exists; false - does not exist</returns>
        bool SettingExists<T, TPropType>(T settings, 
            Expression<Func<T, TPropType>> keySelector, int storeId = 0)
            where T : ISettings, new();

        /// <summary>
        /// Load settings
        /// 加载设置
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier for which settigns should be loaded</param>
        T LoadSetting<T>(int storeId = 0) where T : ISettings, new();
        
        /// <summary>
        /// Save settings object
        /// 保存设置
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new();
        
        /// <summary>
        /// Save settings object
        /// 保存设置（支持表达式语法）
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int storeId = 0, bool clearCache = true) where T : ISettings, new();

        /// <summary>
        /// Save settings object (per store). If the setting is not overridden per storem then it'll be delete
        /// 保存设置。
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="overrideForStore">A value indicating whether to setting is overridden in some store</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        void SaveSettingOverridablePerStore<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            bool overrideForStore, int storeId = 0, bool clearCache = true) where T : ISettings, new();

        /// <summary>
        /// Delete all settings
        /// 删除所有设置
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        void DeleteSetting<T>() where T : ISettings, new();
        
        /// <summary>
        /// Delete settings object
        /// 删除设置
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        void DeleteSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new();

        /// <summary>
        /// Clear cache
        /// 清除缓存
        /// </summary>
        void ClearCache();
    }
}
