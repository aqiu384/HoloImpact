using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Generates modal toggle group from a set of options.
/// <c>TEnum</c> enumerates the options to list.
/// </summary>
public abstract class BaseToggleGroup<TEnum> : MonoBehaviour
    where TEnum : struct, IComparable, IFormattable, IConvertible
{
    public GameObject togglePrefab;
    public TEnum[] excludeOptions;

    [SerializeField]
    private TEnum m_selectedOption;
    private Toggle[] m_toggles = new Toggle[Enum.GetValues(typeof(TEnum)).Length];
    private ToggleGroup m_toggleGroup;

    public delegate void OnValueChangedDelegate(TEnum value);
    public OnValueChangedDelegate onValueChangedDelegate;

    protected virtual void Awake()
    {
        m_toggleGroup = gameObject.AddComponent<ToggleGroup>();

        foreach (var toggle in GetComponentsInChildren<Toggle>())
        {
            Destroy(toggle.gameObject);
        }

        var excludeOptionsSet = new HashSet<TEnum>(excludeOptions);

        foreach (TEnum option in Enum.GetValues(typeof(TEnum)))
        {
            if (excludeOptionsSet.Contains(option))
            {
                continue;
            }

            var toggleObject = Instantiate(togglePrefab);
            var toggle = toggleObject.GetComponent<Toggle>();

            toggleObject.transform.SetParent(transform, false);
            toggle.GetComponentInChildren<Text>().text = option.ToString();

            UnityAction<bool> sayName = (a) => CheckSelectedChanged(a, option);

            toggle.name = option.ToString();
            toggle.isOn = false;
            toggle.group = m_toggleGroup;
            toggle.onValueChanged.AddListener(sayName);

            m_toggles[Convert.ToInt32(option)] = toggle;
        }

        SetSelectedOption(m_selectedOption);
    }

    private void CheckSelectedChanged(bool isOn, TEnum option)
    {
        if (isOn && !m_selectedOption.Equals(option))
        {
            m_selectedOption = option;
            if (onValueChangedDelegate != null) onValueChangedDelegate(m_selectedOption);
        }
    }

    public TEnum GetSelectedOption()
    {
        return m_selectedOption;
    }

    public bool SetSelectedOption(TEnum option)
    {
        var toggle = m_toggles[Convert.ToInt32(option)];
        m_selectedOption = option;
        toggle.isOn = true;
        return true;
    }
}