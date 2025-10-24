using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
public static class SaveLoadService
{
    public static void SaveGame(GameState gameState, string filePath = "savegame.json")
    {
        try
        {
            string json = JsonUtility.ToJson(gameState, true);
            File.WriteAllText(filePath, json);
            Debug.Log("���� ������� ���������!");
        }
        catch (Exception ex)
        {
            Debug.Log($"������ ����������: {ex.Message}");
        }
    }

    public static GameState LoadGame(string filePath = "savegame.json")
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.Log("���� ���������� �� ������, ������� �����");
                return new GameState();
            }

            string json = File.ReadAllText(filePath);
            GameState loadedState = JsonUtility.FromJson<GameState>(json);

            loadedState.Bonuses ??= new List<string>();
            loadedState.Weapons ??= new List<string>();

            return loadedState;
        }
        catch (Exception ex)
        {
            Debug.Log($"������ ��������: {ex.Message}");
            return new GameState();
        }
    }
}
