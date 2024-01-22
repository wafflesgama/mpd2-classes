using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCoinCollector : MonoBehaviour
{
    //Numero de moedas
    public int numOfCoins;


    private PlayerTriggerChecker triggerChecker;
    UEventHandler eventHandler = new UEventHandler();


    private void Awake()
    {
        triggerChecker = GetComponent<PlayerTriggerChecker>();
        numOfCoins = 0;
    }
    void Start()
    {
        triggerChecker.OnTriggered.Subscribe(eventHandler, GrabbedCoin);
    }

    //Funcao invocada quando valor das moedas e atualizado
    public void GrabbedCoin(Transform coin)
    {
        numOfCoins++;

        var coinComponent = coin.GetComponent<Coin>();
        coinComponent.Grabbed();


    }

}
