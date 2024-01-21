using System.Collections;
using System.Collections.Generic;
using Services;
using UI.Model;
using Stonks;
using UnityEngine;
using Utils;

// use this where the view isnt intended to change, or isnt a ui in the traditional sense (like root)
// you almost always want the other class
public abstract class UIController<TStaticView> : IUIController where TStaticView : UIInteractable
{
    public IUIController LogicalParent { get; set; }
    
    protected HashSet<IUIController> Children = new();
    protected UIActiveMutex ActivityMutex;
    
    protected UIDriver UiDriver;
    protected ControllerDatabase ControllerDb;
    protected TStaticView View { get; private set; }

    public bool IsActiveUI => ActivityMutex != null || LogicalParent is { IsActiveUI: true };
    private bool ModelDirty = false;

    public UIController(TStaticView view)
    {
        GetServices();
        View = view;
        ControllerDb.Register(View, this);
    }
    
    public UIController(UIViewTemplate<TStaticView> template, Transform parent)
    {
        GetServices();
        View = GameObject.Instantiate(template.Prefab, parent == null ? UiDriver.Root.transform : parent);
        ControllerDb.Register(View, this);
    }
    
    private void GetServices()
    {
        if (GetType() != typeof(UIDriver) && !ServiceLocator.TryGetService(out UiDriver))
            Debug.LogError($"InputService not found when instantiating {GetType()}");
        
        ControllerDb = ServiceLocator.GetService<ControllerDatabase>();
    }
    
    public virtual void Show()
    {
        View.gameObject.SetActive(true);
    }
    
    public virtual void Hide()
    {
        View.gameObject.SetActive(false);
    }
    
    public virtual void Dispose()
    {
        Close();
    }
    
    public virtual void Close()
    {
        CloseChildren();
        LogicalParent.RemoveChild(this);
        UiDriver.UnregisterForAll(View);
        GameObject.Destroy(View.gameObject);
    }

    protected virtual void CloseChildren()
    {
        // close children from the bottom up
        foreach (IUIController child in new List<IUIController>(Children))
        { 
            child.Close();
        }
    }
    
    public TController AddChild<TController>(TController childController) where TController : IUIController
    {
        if (Children.Contains(childController))
            // idempotent addition, let us know
            return childController;
        Children.Add(childController);
        childController.LogicalParent = this;
        return childController;
    }

    public bool RemoveChild(IUIController childController)
    {
        return Children.Remove(childController);
    }
}

[System.Serializable]
public abstract class UIController<TView, TModel> : IUIController where TView : UIView<TModel> 
                                                                  where TModel : struct, IUIModel
{
    public IUIController LogicalParent { get; set; }
    
    protected HashSet<IUIController> Children = new();
    protected UIActiveMutex ActivityMutex;

    protected ControllerDatabase ControllerDb;
    protected UIDriver UiDriver;
    protected Ticker Ticker;
    protected Timeline Timeline;
    protected Economy Economy;
    protected TView View { get; private set; }
    [SerializeField]
    protected TModel Model;

    public bool IsActiveUI => ActivityMutex != null || LogicalParent is { IsActiveUI: true };
    private bool ModelDirty = false;
    public UIInteractable interactable => View.interactable;

    public UIController(TView view, TModel model = default)
    {
        GetServices();
        View = view;
        
        ControllerDb.Register(View, this);
        Model = model;
        View.UpdateViewWithModel(Model);
    }
    public UIController(UIViewTemplate<TView> template, Transform parent, TModel model = default)
    {
        GetServices();
        View = GameObject.Instantiate(template.Prefab, parent == null ? UiDriver.Root.transform : parent);
        
        ControllerDb.Register(View, this);
        Model = model;
    }

    protected virtual void GetServices()
    {
        if (!ServiceLocator.TryGetService(out UiDriver))
            Debug.LogError($"InputService not found when instantiating {GetType()}");

        ControllerDb = ServiceLocator.GetService<ControllerDatabase>();
        Ticker = ServiceLocator.GetService<Ticker>();
        Timeline = ServiceLocator.GetService<Timeline>();
        Economy = ServiceLocator.GetService<Economy>();
    }

    protected void UpdateViewAtEndOfFrame()
    {
        if (View != null && View.isActiveAndEnabled && !ModelDirty) 
        {
            ModelDirty = true;
            View.StartCoroutine(WaitThenUpdateModel());
        }
    }
    
    // wait until the very last second to batch changes
    // this does mean that updates wont appear until the next frame's rendering
    private IEnumerator WaitThenUpdateModel()
    {
        yield return new WaitForEndOfFrame();
        View.UpdateViewWithModel(Model);
        ModelDirty = false;
    }

    public virtual void Show()
    {
        View.gameObject.SetActive(true);
    }
    
    public virtual void Hide()
    {
        View.gameObject.SetActive(false);
    }
    
    public virtual void Dispose()
    {
        Close();
    }
    
    public virtual void Close()
    {
        if (View.IsDestroyed)
            return;
        
        CloseChildren();
        LogicalParent?.RemoveChild(this);
        UiDriver.UnregisterForAll(View);
        View.Destroy();
    }

    protected virtual void CloseChildren()
    {
        // close children from the bottom up
        foreach (IUIController child in new List<IUIController>(Children))
        { 
            child.Close();
        }
    }
    
    public TController AddChild<TController>(TController childController) where TController : IUIController
    {
        if (Children.Contains(childController))
            // idempotent addition, let us know
            return childController;
        Children.Add(childController);
        childController.LogicalParent = this;
        return childController;
    }
    
    public bool RemoveChild(IUIController childController)
    {
        return Children.Remove(childController);
    }
}
