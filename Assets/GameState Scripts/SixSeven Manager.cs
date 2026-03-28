using UnityEngine;

public class TrollGlobal : MonoBehaviour
{
    // Keeping it static means other scripts can still say "TrollGlobal.Level"
    public static int Level = 0;

    // This ensures the level starts at 0 when you press Play
    void Awake()
    {
        Level = 0;
    }
    private void Update()
    {
        Debug.Log ("Troll level is" + TrollGlobal.Level);
    }
}