using UnityEngine;
using UnityEngine.UIElements;

public class TaleDockEngine : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _rootElement;
    private VisualElement _floatingLayer;
    private TaleMenuWidget _menuWidget;

    private void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        var root = _uiDocument.rootVisualElement;

        _rootElement = root.Q<VisualElement>("root-element");
        _floatingLayer = root.Q<VisualElement>("floating-layer");

        // راه‌اندازی ویجت منوی بالا به صورت ماژولار
        _menuWidget = new TaleMenuWidget();
        _menuWidget.Initialize(root, actionName =>
        {
            Debug.Log($"Menu Action Triggered: {actionName}");
            if (actionName == "New Panel")
            {
                CreateFloatingPanel("Dynamic Panel", 150, 150);
            }
            else if (actionName == "Toggle Theme" || actionName == "Switch Theme (Dark / Light)")
            {
                _rootElement.ToggleInClassList("light-theme");
            }
            else if (actionName == "Exit")
            {
                Application.Quit();
            }
        });

        // دکمه‌های نوار ابزار بالا
        var btnSpawn = root.Q<Button>("btn-spawn-panel");
        if (btnSpawn != null)
        {
            btnSpawn.clicked += () => CreateFloatingPanel("Tale Panel " + Random.Range(10, 99), 120, 120);
        }

        var btnTheme = root.Q<Button>("btn-toggle-theme");
        if (btnTheme != null && _rootElement != null)
        {
            btnTheme.clicked += () => _rootElement.ToggleInClassList("light-theme");
        }

        // ساخت پنل پیش‌فرض اولیه
        CreateFloatingPanel("Explorer / Workspace", 50, 60);
    }

    public VisualElement CreateFloatingPanel(string title, float startX, float startY)
    {
        var panel = new VisualElement();
        panel.AddToClassList("dock-panel");
        panel.style.left = startX;
        panel.style.top = startY;

        var header = new VisualElement();
        header.AddToClassList("panel-header");

        var titleLabel = new Label(title);
        titleLabel.AddToClassList("panel-title");
        header.Add(titleLabel);

        var closeBtn = new Button(() =>
        {
            _floatingLayer.Remove(panel);
        });
        closeBtn.text = "✕";
        closeBtn.AddToClassList("panel-close-btn");
        header.Add(closeBtn);

        panel.Add(header);

        var body = new VisualElement();
        body.AddToClassList("panel-body");
        var placeholderText = new Label("Empty Content Template\n(Load your custom widget here)");
        placeholderText.AddToClassList("placeholder-label");
        body.Add(placeholderText);

        panel.Add(body);

        // مکانیزم درگ کردن روان پنل
        bool isDragging = false;
        Vector2 pointerStartPos = Vector2.zero;

        header.RegisterCallback<PointerDownEvent>(evt =>
        {
            if (evt.button == 0)
            {
                isDragging = true;
                pointerStartPos = evt.position;
                header.CapturePointer(evt.pointerId);
                panel.BringToFront();
                evt.StopPropagation();
            }
        });

        header.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (isDragging)
            {
                Vector2 currentPos = evt.position;
                Vector2 diff = currentPos - pointerStartPos;

                panel.style.left = panel.resolvedStyle.left + diff.x;
                panel.style.top = panel.resolvedStyle.top + diff.y;

                pointerStartPos = currentPos;
                evt.StopPropagation();
            }
        });

        header.RegisterCallback<PointerUpEvent>(evt =>
        {
            if (isDragging)
            {
                isDragging = false;
                header.ReleasePointer(evt.pointerId);
                evt.StopPropagation();
            }
        });

        _floatingLayer.Add(panel);
        return panel;
    }
}