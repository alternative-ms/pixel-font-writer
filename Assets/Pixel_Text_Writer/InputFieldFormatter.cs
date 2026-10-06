using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InputFieldFormatter : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.InputField _inputField;

    [SerializeField]
    private TextMesh _textMesh;

    [SerializeField]
    private UnityEngine.UI.Text _text;

    [SerializeField]
    private float _textPreferredWidth;
    //private Vector2 _actualSize;

    private string modifiedText;

    void Start()
    {
        FixTextPos();
    }

    public void ModifyInput(string text)
    {
        modifiedText = text.Replace(" ", "_").ToUpper();

        if (text != modifiedText)
        {
            int caretPosition = _inputField.caretPosition;

            _inputField.text = modifiedText;

            _inputField.caretPosition = caretPosition;
        }

        if (_textMesh != null) _textMesh.text = modifiedText;
        if (_text != null) _text.text = modifiedText;

        FixTextPos();
    }

    public void FixTextPos()
    {
        if (_text != null)
        {
            Canvas.ForceUpdateCanvases();

            //_actualSize = _text.cachedTextGenerator.rectExtents.size; //v1

            //Canvas.ForceUpdateCanvases();
            //_actualSize = TextGeometryUtility.GetExactPixelSize(_text); // v2

            //_actualSize = new Vector2(_text.preferredWidth, _text.preferredHeight); // v3 // размер симола содержит обводку (смотри на атлас)
            _textPreferredWidth = _text.preferredWidth - 1;

            _text.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Mathf.RoundToInt(_textPreferredWidth / 2), 0);
        }
    }

}
