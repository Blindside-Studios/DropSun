using Microsoft.Web.WebView2.Core;
using Pikouna_Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

namespace DropSun.Model.ViewModels
{
    public class AppSettings : INotifyPropertyChanged
    {
        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppSettings();
                    _instance.Load(); // load settings
                }
                return _instance;
            }
        }
        private static AppSettings _instance;

        public MeasurementUnits Units
        {
            get => _units;
            set
            {
                if (value != _units)
                {
                    _units = value;
                    Save(nameof(Units), (int)value);
                    OnPropertyChanged(nameof(Units));
                }
            }
        }
        private MeasurementUnits _units = MeasurementUnits.GetFromRegion;

        public MeasurementUnits GetMeasurementUnits()
        {
            if (this.Units != MeasurementUnits.GetFromRegion)
            {
                Pikouna_Engine.ApplicationViewModel.Instance.IsUsingImperial = this.Units == MeasurementUnits.Imperial;
                return this.Units;
            }
            else
            {
                Pikouna_Engine.ApplicationViewModel.Instance.IsUsingImperial = !RegionInfo.CurrentRegion.IsMetric;
                return RegionInfo.CurrentRegion.IsMetric ? MeasurementUnits.Metric : MeasurementUnits.Imperial;
            }
        }

        public SunInteractionStyle InteractionStyle
        {
            get => _interactionStyle;
            set
            {
                if (value != _interactionStyle)
                {
                    Pikouna_Engine.ApplicationViewModel.Instance.SunInteractionStyle = value;
                    _interactionStyle = value;
                    Save(nameof(InteractionStyle), (int)value);
                    OnPropertyChanged(nameof(InteractionStyle));
                }
            }
        }
        private SunInteractionStyle _interactionStyle = SunInteractionStyle.Gentle;

        public bool EnablePowerSaver
        {
            get => _enablePowerSaver;
            set
            {
                if (_enablePowerSaver != value)
                {
                    Pikouna_Engine.ApplicationViewModel.Instance.IsBatterySaverEnabled = value;
                    _enablePowerSaver = value;
                    Save(nameof(EnablePowerSaver), value);
                    OnPropertyChanged(nameof(EnablePowerSaver));
                }
            }
        }
        private bool _enablePowerSaver = false;

        public bool ReduceLightningStrikeFlashing
        {
            get => _reduceLightningStrikeFlashing;
            set
            {
                if (_reduceLightningStrikeFlashing != value)
                {
                    _reduceLightningStrikeFlashing = value;
                    ApplicationViewModel.Instance.ReduceThunderstormFlashes = value;
                    Save(nameof(ReduceLightningStrikeFlashing), value);
                    OnPropertyChanged(nameof(ReduceLightningStrikeFlashing));
                }
            }
        }
        private bool _reduceLightningStrikeFlashing = false;

        public bool DisableSidebarRippleEffect
        {
            get => _disableSidebarRippleEffect;
            set
            {
                if (_disableSidebarRippleEffect != value)
                {
                    _disableSidebarRippleEffect = value;
                    Save(nameof(DisableSidebarRippleEffect), value);
                    OnPropertyChanged(nameof(DisableSidebarRippleEffect));
                }
            }
        }
        private bool _disableSidebarRippleEffect = false;

        public bool DisableAllWeatherEffects
        {
            get => _disableAllWeatherEffects;
            set
            {
                if (_disableAllWeatherEffects != value)
                {
                    _disableAllWeatherEffects = value;
                    Save(nameof(DisableAllWeatherEffects), value);
                    Pikouna_Engine.ApplicationViewModel.Instance.CanPlayAnimations = !value;
                    OnPropertyChanged(nameof(DisableAllWeatherEffects));
                }
            }
        }
        private bool _disableAllWeatherEffects = false;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private static void Save(string propertyName, object propertyValue)
        {
            ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
            _localSettings.Values[propertyName] = propertyValue;
        }

        private void Load()
        {
            ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

            // for each existing property, check if there is an appropriate saved setting and if there is, set it
            foreach (var property in typeof(AppSettings).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.Name != nameof(Instance)))
            {
                if (!_localSettings.Values.TryGetValue(property.Name, out object storedValue))
                    continue; // Skip if no saved value exists

                try
                {
                    if (property.PropertyType.IsEnum) 
                        property.SetValue(this, Enum.ToObject(property.PropertyType, storedValue));
                    else property.SetValue(this, Convert.ChangeType(storedValue, property.PropertyType));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load setting {property.Name}: {ex.Message}");
                }
            }

            GetMeasurementUnits(); // run this once to apply to Pikouna Engine
        }

    }

    public enum MeasurementUnits
    {
        GetFromRegion,
        Metric,
        Imperial
    }
}
