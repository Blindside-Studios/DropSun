﻿#pragma checksum "C:\Users\nicol\OneDrive\Documents\GitHub\DropSun\DropSun\Views\Conditions\Rendered\Sunny.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "748103DE5979D77E34DFA6CD1E38A64166CCA236352AB70E81A22EA6134264A0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DropSun.Views.Conditions.Rendered
{
    partial class Sunny : 
        global::Microsoft.UI.Xaml.Controls.Page, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2309")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_Microsoft_UI_Xaml_UIElement_Translation(global::Microsoft.UI.Xaml.UIElement obj, global::System.Numerics.Vector3 value)
            {
                obj.Translation = value;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2309")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class Sunny_obj1_Bindings :
            global::Microsoft.UI.Xaml.Markup.IDataTemplateComponent,
            global::Microsoft.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Microsoft.UI.Xaml.Markup.IComponentConnector,
            ISunny_Bindings
        {
            private global::DropSun.Views.Conditions.Rendered.Sunny dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Microsoft.UI.Xaml.Controls.Frame obj4;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj4TranslationDisabled = false;

            private Sunny_obj1_BindingsTracking bindingsTracking;

            public Sunny_obj1_Bindings()
            {
                this.bindingsTracking = new Sunny_obj1_BindingsTracking(this);
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 14 && columnNumber == 84)
                {
                    isobj4TranslationDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 4: // Views\Conditions\Rendered\Sunny.xaml line 14
                        this.obj4 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Frame>(target);
                        break;
                    default:
                        break;
                }
            }
                        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2309")]
                        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target) 
                        {
                            return null;
                        }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                nextPhase = -1;
            }

            public void Recycle()
            {
                return;
            }

            // ISunny_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
                this.bindingsTracking.ReleaseAllListeners();
                this.initialized = false;
            }

            public void DisconnectUnloadedObject(int connectionId)
            {
                throw new global::System.ArgumentException("No unloadable elements to disconnect.");
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                this.bindingsTracking.ReleaseAllListeners();
                if (newDataRoot != null)
                {
                    this.dataRoot = global::WinRT.CastExtensions.As<global::DropSun.Views.Conditions.Rendered.Sunny>(newDataRoot);
                    return true;
                }
                return false;
            }

            public void Activated(object obj, global::Microsoft.UI.Xaml.WindowActivatedEventArgs data)
            {
                this.Initialize();
            }

            public void Loading(global::Microsoft.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::DropSun.Views.Conditions.Rendered.Sunny obj, int phase)
            {
                this.Update_DropSun_Model_ViewModels_ViewRenderingModel_Instance(global::DropSun.Model.ViewModels.ViewRenderingModel.Instance, phase);
            }
            private void Update_DropSun_Model_ViewModels_ViewRenderingModel_Instance(global::DropSun.Model.ViewModels.ViewRenderingModel obj, int phase)
            {
                this.bindingsTracking.UpdateChildListeners_DropSun_Model_ViewModels_ViewRenderingModel_Instance(obj);
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | DATA_CHANGED | (1 << 0))) != 0)
                    {
                        this.Update_DropSun_Model_ViewModels_ViewRenderingModel_Instance_SunTranslation(obj.SunTranslation, phase);
                    }
                }
            }
            private void Update_DropSun_Model_ViewModels_ViewRenderingModel_Instance_SunTranslation(global::System.Numerics.Vector3 obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED | DATA_CHANGED)) != 0)
                {
                    // Views\Conditions\Rendered\Sunny.xaml line 14
                    if (!isobj4TranslationDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_UIElement_Translation(this.obj4, obj);
                    }
                }
            }

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2309")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
            private class Sunny_obj1_BindingsTracking
            {
                private global::System.WeakReference<Sunny_obj1_Bindings> weakRefToBindingObj; 

                public Sunny_obj1_BindingsTracking(Sunny_obj1_Bindings obj)
                {
                    weakRefToBindingObj = new global::System.WeakReference<Sunny_obj1_Bindings>(obj);
                }

                public Sunny_obj1_Bindings TryGetBindingObject()
                {
                    Sunny_obj1_Bindings bindingObject = null;
                    if (weakRefToBindingObj != null)
                    {
                        weakRefToBindingObj.TryGetTarget(out bindingObject);
                        if (bindingObject == null)
                        {
                            weakRefToBindingObj = null;
                            ReleaseAllListeners();
                        }
                    }
                    return bindingObject;
                }

                public void ReleaseAllListeners()
                {
                    UpdateChildListeners_DropSun_Model_ViewModels_ViewRenderingModel_Instance(null);
                }

                public void PropertyChanged_DropSun_Model_ViewModels_ViewRenderingModel_Instance(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
                {
                    Sunny_obj1_Bindings bindings = TryGetBindingObject();
                    if (bindings != null)
                    {
                        string propName = e.PropertyName;
                        global::DropSun.Model.ViewModels.ViewRenderingModel obj = sender as global::DropSun.Model.ViewModels.ViewRenderingModel;
                        if (global::System.String.IsNullOrEmpty(propName))
                        {
                            if (obj != null)
                            {
                                bindings.Update_DropSun_Model_ViewModels_ViewRenderingModel_Instance_SunTranslation(obj.SunTranslation, DATA_CHANGED);
                            }
                        }
                        else
                        {
                            switch (propName)
                            {
                                case "SunTranslation":
                                {
                                    if (obj != null)
                                    {
                                        bindings.Update_DropSun_Model_ViewModels_ViewRenderingModel_Instance_SunTranslation(obj.SunTranslation, DATA_CHANGED);
                                    }
                                    break;
                                }
                                default:
                                    break;
                            }
                        }
                    }
                }
                private global::DropSun.Model.ViewModels.ViewRenderingModel cache_DropSun_Model_ViewModels_ViewRenderingModel_Instance = null;
                public void UpdateChildListeners_DropSun_Model_ViewModels_ViewRenderingModel_Instance(global::DropSun.Model.ViewModels.ViewRenderingModel obj)
                {
                    if (obj != cache_DropSun_Model_ViewModels_ViewRenderingModel_Instance)
                    {
                        if (cache_DropSun_Model_ViewModels_ViewRenderingModel_Instance != null)
                        {
                            ((global::System.ComponentModel.INotifyPropertyChanged)cache_DropSun_Model_ViewModels_ViewRenderingModel_Instance).PropertyChanged -= PropertyChanged_DropSun_Model_ViewModels_ViewRenderingModel_Instance;
                            cache_DropSun_Model_ViewModels_ViewRenderingModel_Instance = null;
                        }
                        if (obj != null)
                        {
                            cache_DropSun_Model_ViewModels_ViewRenderingModel_Instance = obj;
                            ((global::System.ComponentModel.INotifyPropertyChanged)obj).PropertyChanged += PropertyChanged_DropSun_Model_ViewModels_ViewRenderingModel_Instance;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2309")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // Views\Conditions\Rendered\Sunny.xaml line 12
                {
                    this.ContentGrid = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Grid>(target);
                }
                break;
            case 3: // Views\Conditions\Rendered\Sunny.xaml line 13
                {
                    this.BlueSkyFrame = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Frame>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Frame)this.BlueSkyFrame).SizeChanged += this.BlueSkyFrame_SizeChanged;
                }
                break;
            case 4: // Views\Conditions\Rendered\Sunny.xaml line 14
                {
                    this.SunFrame = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Frame>(target);
                }
                break;
            case 5: // Views\Conditions\Rendered\Sunny.xaml line 15
                {
                    this.GrassFrame = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Frame>(target);
                }
                break;
            case 6: // Views\Conditions\Rendered\Sunny.xaml line 16
                {
                    this.GroundFrame = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Frame>(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2309")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            switch(connectionId)
            {
            case 1: // Views\Conditions\Rendered\Sunny.xaml line 2
                {                    
                    global::Microsoft.UI.Xaml.Controls.Page element1 = (global::Microsoft.UI.Xaml.Controls.Page)target;
                    Sunny_obj1_Bindings bindings = new Sunny_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                    global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(element1, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

