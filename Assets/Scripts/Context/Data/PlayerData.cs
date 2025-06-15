using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Header("Prefab Info")]
    public string prefabPath;

    [Header("Player Stats")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _currentMana;
    [SerializeField] private int _level = 1;
    [SerializeField] private float _experience;
    [SerializeField] private int _coin;

    [Header("Max Stats")]
    private int _maxHealth = 100;
    private int _maxMana = 100;
    private int _maxLevel = 100;
    private int _maxExperience = 100;
    private int _maxCoin = 999_999;

    //--------------------------------------//
    public float CurrentHealth
    {
        get => _currentHealth;
        set => _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
    }

    public float CurrentMana
    {
        get => _currentMana;
        set => _currentMana = Mathf.Clamp(value, 0, _maxMana);
    }

    public int Level
    {
        get => _level;
        set => _level = Mathf.Clamp(value, 1, _maxLevel);
    }

    public float Experience
    {
        get => _experience;
        set => _experience = value;
    }

    public int Coin
    {
        get => _coin;
        set => _coin = Mathf.Clamp(value, 0, _maxCoin);
    }

    //--------------------------------------//
    public int MaxHealth => _maxHealth;
    public int MaxMana => _maxMana;
    public int MaxLevel => _maxLevel;
    public int MaxExperience => _maxExperience;
    public int MaxCoin => _maxCoin;

    //--------------------------------------//
    public void SetMaxHealth(int value) => _maxHealth = value;
    public void SetMaxMana(int value) => _maxMana = value;
    public void SetMaxLevel(int value) => _maxLevel = value;
    public void SetMaxExperience(int levelMultiplier) => _maxExperience *= levelMultiplier;

    //--------------------------------------//
}

[Serializable]
public class PlayerStateInScene
{
    public bool isPlayerExist;
    public float posX;
    public float posY;
    public float posZ;
}