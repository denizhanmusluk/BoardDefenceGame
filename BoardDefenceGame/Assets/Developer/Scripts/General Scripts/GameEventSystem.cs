using System;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public static event Action RPG_Start, RPG_End, GameUpdate, GameFixedUpdate, GunStart, GunStop, GameFail , GameWin;

    private void Awake() => ResetEvents();

    private void Update() => GameUpdate?.Invoke();
    private void FixedUpdate() => GameFixedUpdate?.Invoke();

    public void ResetEvents()
    {
        RPG_Start = RPG_End = GameUpdate = GameFixedUpdate = GunStart = GunStop = GameFail = GameWin = null;
    }

    public static void RPGStartEvent() => RPG_Start?.Invoke();
    public static void RPGEndEvent() => RPG_End?.Invoke();
    public static void GunStartEvent() => GunStart?.Invoke();
    public static void GunStopEvent() => GunStop?.Invoke();
    public static void GameFailEvent() => GameFail?.Invoke();
    public static void GameWinEvent() => GameWin?.Invoke();
}
