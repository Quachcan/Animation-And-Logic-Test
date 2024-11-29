using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCombo", menuName = "Combat/Combo")]
public class Combo : ScriptableObject
{
    public string comboName; 
    public List<string> attackTriggers; 
    public float comboTime = 0.5f;
    public float comboCoolDownTime = 1.5f;
}
