// UIConstants.cs: shared colour palette and layout-building helpers (MakeRect, MakeImage,
// MakeTMP, etc.) used by SceneBuilder.cs and every LayoutBuilders/*.cs script.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UIConstants
{
    // --- Colour Palette ---
    // All UI colours defined in one place. Change a hex here and it updates everywhere.
    public static readonly Color ColBg          = Hex("#081C15"); // dark forest background
    public static readonly Color ColSurface     = Hex("#1B4332"); // panels, cards, input fields
    public static readonly Color ColButton      = Hex("#D4A373"); // earth-tone primary button
    public static readonly Color ColButtonText  = Hex("#1B4332"); // dark text on buttons
    public static readonly Color ColTextPrimary = Hex("#F1FAEE"); // headings and body text
    public static readonly Color ColTextSecondary = Hex("#000000"); // rarity badge text, card name, and other secondary text
    public static readonly Color ColTextMuted   = Hex("#95B8A0"); // placeholder and subtitle text
    public static readonly Color ColGold        = Hex("#E9C46A"); // gold for coins and rarity diamonds
    public static readonly Color ColInputBg     = Hex("#102215"); // input fields, darker than ColSurface

    // --- Fonts ---
    // Loaded from Assets/Fonts/ at build time.
    // Cinzel is used for titles only; Poppins is used for everything else.
    static TMP_FontAsset s_Poppins;
    static TMP_FontAsset s_Cinzel;

    // Create a RectTransform GameObject parented to `parent`
    public static RectTransform MakeRect(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.localScale = Vector3.one;
        rt.localPosition = Vector3.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        return rt;
    }

    // Create an Image under parent, optional sprite and colour
    public static Image MakeImage(Transform parent, string name, Sprite sprite = null, Color? color = null)
    {
        var rt = MakeRect(parent, name);
        var img = rt.gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color ?? Color.white;
        // default to preserving native size if sprite present
        if (sprite != null)
        {
            rt.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
        }
        return img;
    }

    // Stretch RectTransform to fill its parent (anchors 0..1, zero offsets)
    public static void Stretch(RectTransform rt)
    {
        if (rt == null) return;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Set anchor min/max and size. Position is zeroed; set anchoredPosition separately if needed.
    public static void SetAnchored(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
    {
        if (rt == null) return;
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
    }

    // Find a component by path under root. Supports slash-separated paths (e.g. "FormCard/EmailInput").
    public static T Find<T>(Transform root, string path) where T : Component
    {
        if (root == null) return null;
        var t = root.Find(path);
        return t != null ? t.GetComponent<T>() : null;
    }

    // Parse HTML hex colour strings (#RRGGBB or #RRGGBBAA, with or without leading '#')
    public static Color Hex(string hex)
    {
        if (string.IsNullOrEmpty(hex)) return Color.magenta;
        if (!hex.StartsWith("#")) hex = "#" + hex;
        if (ColorUtility.TryParseHtmlString(hex, out Color c)) return c;
        Debug.LogWarning($"UIHelpers.Hex: failed to parse color '{hex}', returning magenta.");
        return Color.magenta;
    }

    // Warn if a serialized/unwired reference is null
    public static void WarnIfUnwired(Object obj, string fieldName)
    {
        if (obj == null)
        {
            var context = Selection.activeObject;
            if (context != null)
                Debug.LogWarning($"Unwired reference: '{fieldName}' (context: {context.name})", context);
            else
                Debug.LogWarning($"Unwired reference: '{fieldName}'");
        }
    }

    // Checks a SerializedObject for null objectReference properties and warns for each missing one.
    public static void WarnIfUnwired(SerializedObject so, params string[] props)
    {
        foreach (var prop in props)
        {
            var p = so.FindProperty(prop);
            if (p == null || p.objectReferenceValue == null)
                Debug.LogWarning($"[SceneBuilder] '{prop}' on {so.targetObject.name} was not wired.");
        }
    }

    public static TextMeshProUGUI MakeTMP(Transform parent, string name, string text, float fontSize, Color color, FontStyles style, TMP_FontAsset font = null)
    {
        var rt = MakeRect(parent, name);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.fontStyle = style;
        if (font != null) tmp.font = font;
        return tmp;
    }

    // Creates a TMP_InputField with a text area, placeholder, and text child.
    public static TMP_InputField MakeInputField(Transform parent, string name, string placeholder, bool isPassword, TMP_FontAsset font = null)
    {
        var root = MakeRect(parent, name);
        var bg = root.gameObject.AddComponent<Image>();
        bg.color = ColInputBg;
        var field = root.gameObject.AddComponent<TMP_InputField>();

        var textArea = MakeRect(root, "Text Area");
        textArea.anchorMin = Vector2.zero;
        textArea.anchorMax = Vector2.one;
        textArea.offsetMin = new Vector2(10, 6);
        textArea.offsetMax = new Vector2(-10, -6);
        textArea.gameObject.AddComponent<RectMask2D>();

        var placeholderRt = MakeRect(textArea, "Placeholder");
        placeholderRt.anchorMin = Vector2.zero;
        placeholderRt.anchorMax = Vector2.one;
        placeholderRt.sizeDelta = Vector2.zero;
        var placeholderTmp = placeholderRt.gameObject.AddComponent<TextMeshProUGUI>();
        placeholderTmp.text = placeholder;
        placeholderTmp.color = ColTextMuted;
        placeholderTmp.fontSize = 14;
        if (font != null) placeholderTmp.font = font;

        var textRt = MakeRect(textArea, "Text");
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        var textTmp = textRt.gameObject.AddComponent<TextMeshProUGUI>();
        textTmp.color = ColTextPrimary;
        textTmp.fontSize = 14;
        if (font != null) textTmp.font = font;

        field.textViewport = textArea;
        field.textComponent = textTmp;
        field.placeholder = placeholderTmp;
        if (isPassword) field.contentType = TMP_InputField.ContentType.Password;

        return field;
    }

    public static (GameObject, TextMeshProUGUI) MakeButton(Transform parent, string name, string label, float fontSize, TMP_FontAsset font = null, Color? bgColor = null, Color? textColor = null)
    {
        var rt = MakeRect(parent, name);
        var bg = rt.gameObject.AddComponent<Image>();
        bg.color = bgColor ?? ColButton;
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = bg;

        var textRt = MakeRect(rt, "Text");
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        var textTmp = textRt.gameObject.AddComponent<TextMeshProUGUI>();
        textTmp.text = label;
        textTmp.fontSize = fontSize;
        textTmp.color = textColor ?? ColButtonText;
        textTmp.alignment = TextAlignmentOptions.Center;
        textTmp.fontStyle = FontStyles.Bold;
        if (font != null) textTmp.font = font;

        return (rt.gameObject, textTmp);
    }

    // Bordered button: a slightly larger background Image (the border) behind an inset
    // dark Fill image, same peeking-border trick used for rarity/answer card outlines.
    public static (GameObject, TextMeshProUGUI) MakeOutlineButton(Transform parent, string name, string label, float fontSize, TMP_FontAsset font = null, Color? borderColor = null, Color? textColor = null)
    {
        var rt = MakeRect(parent, name);
        var border = rt.gameObject.AddComponent<Image>();
        border.color = borderColor ?? ColGold;

        var fillRt = MakeRect(rt, "Fill");
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = new Vector2(2, 2);
        fillRt.offsetMax = new Vector2(-2, -2);
        var fill = fillRt.gameObject.AddComponent<Image>();
        fill.color = ColBg;

        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = fill;

        var textRt = MakeRect(fillRt, "Text");
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        var textTmp = textRt.gameObject.AddComponent<TextMeshProUGUI>();
        textTmp.text = label;
        textTmp.fontSize = fontSize;
        textTmp.color = textColor ?? ColTextPrimary;
        textTmp.alignment = TextAlignmentOptions.Center;
        textTmp.fontStyle = FontStyles.Bold;
        if (font != null) textTmp.font = font;

        return (rt.gameObject, textTmp);
    }

    public static (GameObject, TextMeshProUGUI) MakeLinkButton(Transform parent, string name, string label, float fontSize, TMP_FontAsset font = null)
    {
        var rt = MakeRect(parent, name);
        var bg = rt.gameObject.AddComponent<Image>();
        bg.color = Color.clear;
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = bg;

        var textRt = MakeRect(rt, "Text");
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        var textTmp = textRt.gameObject.AddComponent<TextMeshProUGUI>();
        textTmp.text = label;
        textTmp.fontSize = fontSize;
        textTmp.color = ColTextMuted;
        textTmp.alignment = TextAlignmentOptions.Center;
        textTmp.richText = true;
        if (font != null) textTmp.font = font;

        return (rt.gameObject, textTmp);
    }

    // Builds a fully-wired TMP_Dropdown. A label + background alone isn't enough: TMP_Dropdown
    // needs the Template/Viewport/Content/Item subtree to show its popup list, so this delegates
    // to TMPro's own TMP_DefaultControls (the same code Unity's UI menu uses) and then restyles
    // the result to match this project's palette.
    public static TMP_Dropdown MakeDropdown(Transform parent, string name, string[] options, TMP_FontAsset font = null)
    {
        GameObject go = TMP_DefaultControls.CreateDropdown(new TMP_DefaultControls.Resources());
        go.name = name;
        var rt = (RectTransform)go.transform;
        rt.SetParent(parent, false);
        rt.localScale = Vector3.one;

        var dropdown = go.GetComponent<TMP_Dropdown>();
        go.GetComponent<Image>().color = ColInputBg;

        var labelTmp = (TextMeshProUGUI)dropdown.captionText;
        labelTmp.color = ColTextPrimary;
        labelTmp.fontSize = 13;
        if (font != null) labelTmp.font = font;

        // No dropdown-arrow sprite asset exists yet: swap the default Image arrow for a "▼"
        // glyph, same convention as the sort-direction button's text-glyph icon.
        var arrow = (RectTransform)rt.Find("Arrow");
        Object.DestroyImmediate(arrow.GetComponent<Image>());
        var arrowTmp = arrow.gameObject.AddComponent<TextMeshProUGUI>();
        arrowTmp.text = "▼";
        arrowTmp.fontSize = 10;
        arrowTmp.color = ColTextMuted;
        arrowTmp.alignment = TextAlignmentOptions.Center;
        if (font != null) arrowTmp.font = font;

        var template = (RectTransform)rt.Find("Template");
        template.GetComponent<Image>().color = ColSurface;
        template.sizeDelta = new Vector2(0, 28 * options.Length);
        template.Find("Viewport").GetComponent<Image>().color = ColSurface;
        var item = (RectTransform)template.Find("Viewport/Content/Item");
        item.sizeDelta = new Vector2(item.sizeDelta.x, 28);
        template.Find("Viewport/Content/Item/Item Background").GetComponent<Image>().color = ColSurface;
        template.Find("Viewport/Content/Item/Item Checkmark").GetComponent<Image>().color = ColGold;

        var itemLabelTmp = (TextMeshProUGUI)dropdown.itemText;
        itemLabelTmp.color = ColTextPrimary;
        itemLabelTmp.fontSize = 13;
        itemLabelTmp.rectTransform.offsetMin = new Vector2(26, itemLabelTmp.rectTransform.offsetMin.y);
        if (font != null) itemLabelTmp.font = font;

        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>(options));

        return dropdown;
    }
}