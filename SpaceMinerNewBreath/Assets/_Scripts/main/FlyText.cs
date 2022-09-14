using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// An enum to track the possible states of a text
public enum eFSState
{
    idle,
    pre,
    active,
    post
}
public class FlyText : MonoCache
{
    private eFSState _state=eFSState.idle;
    private List<Vector2> _bezierPts; // Bézier points for movement 
    private List<Color> _colors;
    private List<float> _fontSizes; // Bézier points for font scaling 
    private List<Vector2> _rectSizes;
    private float _timeStart = -1f;
    private float _timeDuration = 1f;
    private string _easingCurve = Easing.InOut; // Uses Easing in Utils.cs

    private RectTransform _rectTrans;
    private Text _txt;
    private LocalizedText _localizedText;
    private bool IsCanvasRenderModeWorldSpace = false;
    private GameObject _reportFinsihTo;
    private string _reportFinishFuncName;
    /// <summary>
    /// This public method set colors List for text and in futute color will changed by Bezier
    /// </summary>
    /// <param name="colorList">List of colors for text</param>
    public void SetColorChange(List<Color> colorList)
    {
        _colors = colorList;
        if (colorList.Count >= 1) _txt.color = colorList[0];
    }
    /// <summary>
    /// This public method set rectSize List for text and in future texts' rectSize will changed by Bezier  
    /// </summary>
    /// <param name="rectSize">List of rectSizes(Vector2)</param>
    public void SetRectSize(List<Vector2> rectSize)
    {
        _rectSizes = rectSize;
        if (_rectTrans != null && rectSize.Count == 1)
        {
            _rectTrans.sizeDelta = rectSize[0];
        }
        return;

    }
    /// <summary>
    /// This method calle when FlyText will be instance or pull from list pullMono  
    /// </summary>
    /// <param name="ePts"> this List include possitions that text will following with Bezier</param>
    /// <param name="curve">this curve set with helpp class Easing and curve control text displacement</param>
    /// <param name="eTimeS">time when main behaviour of text start act</param>
    /// <param name="eTimeD">time when main behaviour of text end act</param>
    /// <param name="finsihTo">This parram indicate on varriable which take info when texts' action will be end</param>
    /// <param name="finishFuncName">This param indicate on fucntion of varriable</param>
    /// <param name="isLocalizedText">This param set LocalizedText fot text if will be translating </param>
    /// <param name="localizedTextKey">If text having LocalizedText, then set the key for LocalizedText </param>
    /// <returns>Return FlyText with preset setting</returns>
    public FlyText Init(List<Vector2> ePts, string curve,float eTimeS = 0, float eTimeD = 1, GameObject finsihTo = null,string finishFuncName="", bool isLocalizedText = false, string localizedTextKey="Default") {
        _rectTrans = GetComponent<RectTransform>();
        _rectTrans.anchoredPosition = Vector2.zero;
        _txt = GetComponent<Text>();
        _bezierPts = new List<Vector2>(ePts);
        _easingCurve = curve;
        _reportFinsihTo = finsihTo;
        _reportFinishFuncName = finishFuncName;
        if (isLocalizedText) this.gameObject.AddComponent<LocalizedText>().SetKey(localizedTextKey);
        if (ePts.Count == 1)
        { // If there's only one point
            // ...then just go there.
            transform.position = ePts[0];
            return this;
        }

        // If eTimeS is the default, just start at the current time
        if (eTimeS == 0) eTimeS = Time.time;
        _timeStart = eTimeS;
        _timeDuration = eTimeD;
        _state = eFSState.pre; // Set it to the pre state, ready to start moving
        return this;
    }
   /// <summary>
   /// Set default text 
   /// </summary>
   /// <param name="text"></param>
    public void SetCommonText(string text) {
        _txt.text = text;
    }

    /// <summary>
    /// This method execute all operations conentcted with Bezier
    /// </summary>
    public override void OnTick() {
        // If this is not moving, just return
        if (_state == eFSState.idle) return;

        // Get u from the current time and duration
        // u ranges from 0 to 1 (usually)
        float u = (Time.time - _timeStart) / _timeDuration;
        // Use Easing class from Utils to curve the u value
        float uC = Easing.Ease(u, _easingCurve);
        if (u < 0)
        { // If u<0, then we shouldn't move yet.
            _state = eFSState.pre;
            _txt.enabled = false; // Hide the score initially
        }
        else
        {
            if (u >= 1)
            {
                if (_reportFinsihTo != null)
                {
                    if (this.gameObject.GetComponent<LocalizedText>() != null) {
                        Destroy(this.gameObject.GetComponent<LocalizedText>());
                    }
                    _reportFinsihTo.SendMessage(_reportFinishFuncName);
                }
                this.gameObject.SetActive(false);
            }
            else
            {
                
                // 0<=u<1, which means that this is active and moving
                _state = eFSState.active;
            }

            // Use Bézier curve to move this to the right point
            Vector2 pos = Utils.Bezier(uC, _bezierPts);
            if (IsCanvasRenderModeWorldSpace)
            {
                // RectTransform anchors can be used to position UI objects relative
                //   to total size of the screen
                _rectTrans.anchorMin = _rectTrans.anchorMax = pos;
            }
            else
            {
                transform.position = pos;
            }
            if (_colors != null && _colors.Count > 1) {
                _txt.color = Utils.Bezier(uC,_colors);
            }
            if (_fontSizes != null && _fontSizes.Count > 0)
            {
                // If fontSizes has values in it
                // ...then adjust the fontSize of this GUIText
                    int size = Mathf.RoundToInt(Utils.Bezier(uC, _fontSizes));
                    GetComponent<Text>().fontSize = size;
            }
            if (_rectSizes != null && _rectSizes.Count > 1) {
                Vector2 size = Utils.Bezier(uC, _rectSizes);
                _rectTrans.sizeDelta = size;
                
            }
        }
    }

    protected override void OnEnable ()
    {
        base.OnEnable();
    }
}
