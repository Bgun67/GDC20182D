using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Confirm_Panel : MonoBehaviour
{
    static Confirm_Panel instance;
    public Text warningText;
    public Text confirmText;
    public Text denyText;
    public Action confirmAction;
    public Action denyAction;
    #region constructors
    public Confirm_Panel(string _warning, Action _action)
    {
        instance = ((GameObject)Instantiate(Resources.Load("UI/Confirm Panel"))).GetComponent<Confirm_Panel>();
        instance.confirmAction = _action;
        instance.warningText.text = _warning;
        Destroy (this);
    }
    public Confirm_Panel(string _warning, Action _action, string _confirmText, string _denyText)
    {
        instance = ((GameObject)Instantiate(Resources.Load("UI/Confirm Panel"))).GetComponent<Confirm_Panel>();
        instance.confirmAction = _action;
        instance.warningText.text = _warning;
        instance.confirmText.text = _confirmText;
        instance.denyText.text = _denyText;
        Destroy(this);
    }
    public Confirm_Panel(string _warning, Action _confirmAction, Action _denyAction, string _confirmText, string _denyText)
    {
        instance = ((GameObject)Instantiate(Resources.Load("UI/Confirm Panel"))).GetComponent<Confirm_Panel>();
        instance.confirmAction = _confirmAction;
        instance.denyAction = _denyAction;
        instance.warningText.text = _warning;
        instance.confirmText.text = _confirmText;
        instance.denyText.text = _denyText;
        Destroy(this);
    }
    #endregion

    // Update is called once per frame
    public void Confirm()
    {
        confirmAction();
    }
    public void Deny()
    {
        if(denyAction != null)
        {
            denyAction();
        }
        Destroy(instance.gameObject);
    }
}
