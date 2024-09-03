using UnityEngine;

public class TestButtonToAddOneRes : MonoBehaviour {
    public ResourceData resource;
    public void Clicked() {
        ResourceManager.inst.AddResource(new Resource(resource, 1));
    }
}
