using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public const int EMPTY = 0;

    public Text label;

    private int _number;
    private bool _locked;
    private bool _invalid;
    private bool _focus;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        RefreshColor();
    }

    public int number
    {
        get
        {
            return _number;
        }
        set
        {
            _number = value;
            if (value == EMPTY)
                label.text = "";
            else
                label.text = value.ToString();
        }
    }
    public bool isEmpty
    {
        get
        {
            return _number == 0;
        }
    }
    public bool invalid
    {
        get
        {
            return _invalid;
        }
        set
        {
            _invalid = value;
            RefreshColor();
        }
    }
    public bool locked
    {
        get
        {
            return _locked;
        }
        set
        {
            _locked = value;
            RefreshColor();
        }
    }
    public bool focused
    {
        get
        {
            return _focus;
        }
        set
        {
            _focus = value;
            RefreshColor();
        }
    }

    private void RefreshColor()
    {
        if (_invalid)
            _image.color = Color.red;
        else if (_focus)
            _image.color = Color.yellow;
        else if (_locked)
            _image.color = new Color(0.75f, 0.75f, 0.75f);
        else
            _image.color = Color.white;
    }
}
