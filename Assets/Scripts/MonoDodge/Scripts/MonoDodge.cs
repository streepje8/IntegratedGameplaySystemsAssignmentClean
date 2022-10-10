using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public abstract class MonoDodge
{
    private MonoDodgeComponent MDC;

    //The following two properties ignore the naming conventions for consistency with unity
    [JsonIgnore]
    public Transform transform
    {
        get
        {
            if (MDC == null) return null;
            return MDC.transform;
        }
    }

    [JsonIgnore]
    public GameObject gameObject
    {
        get { return MDC.gameObject; }
    }
    
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update() { }

    public virtual void OnDrawGizmos() { }

    public virtual void OnRenderImage(RenderTexture src, RenderTexture dest) { Graphics.Blit(src,dest); }
    
    public void SetMDC(MonoDodgeComponent MDC) => this.MDC = MDC;

    public T GetComponent<T>() where T : Component
    {
        if(gameObject == null) Debug.LogWarning("Requested a component from a null object!");
        return gameObject.GetComponent<T>();
    }
    
    public T GetDodge<T>() where T : MonoDodge
    {
        if(gameObject == null) Debug.LogWarning("Requested a dodge from a null object!");
        return gameObject.GetDodge<T>();
    }
    
}
