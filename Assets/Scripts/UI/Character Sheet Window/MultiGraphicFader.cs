using UnityEngine;
using UnityEngine.UI;

public class MultiGraphicFader : MonoBehaviour
{
    [SerializeField]
    private Graphic[] graphics;

    /// <summary>
    /// Sets all the alphas of all the graphics.
    /// </summary>
    /// <param name="alpha">in range of [1,0]</param>
    public void SetAlphas(float alpha)
    {
        foreach(Graphic graphic in graphics)
        {
            Color copy = graphic.color;
            copy.a = alpha;
            graphic.color = copy;
        }
    }
}
