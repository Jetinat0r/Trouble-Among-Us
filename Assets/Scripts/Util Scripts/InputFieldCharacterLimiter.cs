using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputFieldCharacterLimiter : MonoBehaviour
{
    [SerializeField]
    private int maxCharacters;
    private void Start()
    {
        GetComponent<TMP_InputField>().characterLimit = maxCharacters;
    }
}
