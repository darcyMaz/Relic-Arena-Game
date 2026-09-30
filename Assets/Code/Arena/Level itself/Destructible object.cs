using UnityEngine;

public class Destructibleobject : MonoBehaviour
{
    [SerializeField] private AudioClip _DestructionSound1;
    [SerializeField] private AudioClip _DestructionSound2; 
    [SerializeField] private string _DestructionAnimation;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
   
     void OnTriggerEnter(Collider other)
       {
    if (other.CompareTag("Player1"))
      return;
    
      DestroySelf();
       
       }

       public void DestroySelf(){
    AudioClip clip = Random.value < 0.5f ? _DestructionSound1 : _DestructionSound2;
    if (clip != null){
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
    Destroy(gameObject);
}
}

