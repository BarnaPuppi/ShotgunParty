using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerManager))]
public class PlayerEditor : Editor
{
    PowerupSpawner.PowerupType chosenPowerup;
    
    public override void OnInspectorGUI()
    {
        PlayerManager playerManager = (PlayerManager)target;
        
        base.OnInspectorGUI();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Change Color")) {
            playerManager.ChangeColor();
        }
        
        if (GUILayout.Button("Reset Percentage")) {
            playerManager.player.percentage = 0f;
        } 
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Reset Position")) {
            playerManager.Respawn();
        }
    }
}
