using UnityEngine.UIElements;

public class TaleMenuWidget
{
    private VisualElement _rootMenuContainer;
    private VisualElement _activeDropdown;

    public void Initialize(VisualElement root, System.Action<string> onMenuAction)
    {
        _rootMenuContainer = root.Q<VisualElement>("menu-bar-root");

        // تعریف منوها و ساب‌منوها به صورت ساختاریافته
        RegisterMenu("menu-file", "dropdown-file", onMenuAction);
        RegisterMenu("menu-edit", "dropdown-edit", onMenuAction);
        RegisterMenu("menu-view", "dropdown-view", onMenuAction);
        RegisterMenu("menu-help", "dropdown-help", onMenuAction);

        // بستن منوها با کلیک در فضای خالی صفحه
        root.RegisterCallback<PointerDownEvent>(evt =>
        {
            CloseAllDropdowns();
        }, TrickleDown.TrickleDown);
    }

    private void RegisterMenu(string btnName, string dropdownName, System.Action<string> onAction)
    {
        var btn = _rootMenuContainer.Q<Button>(btnName);
        var dropdown = _rootMenuContainer.Q<VisualElement>(dropdownName);

        if (btn != null && dropdown != null)
        {
            btn.clicked += () =>
            {
                bool wasHidden = dropdown.ClassListContains("dropdown-hidden");
                CloseAllDropdowns();
                if (wasHidden)
                {
                    dropdown.RemoveFromClassList("dropdown-hidden");
                }
            };

            // ثبت رویداد برای آیتم‌های داخل ساب‌منو
            foreach (var child in dropdown.Children())
            {
                if (child is Button itemBtn)
                {
                    itemBtn.clicked += () =>
                    {
                        onAction?.Invoke(itemBtn.text);
                        CloseAllDropdowns();
                    };
                }
            }
        }
    }

    public void CloseAllDropdowns()
    {
        if (_rootMenuContainer == null) return;
        foreach (var dropdown in _rootMenuContainer.Query<VisualElement>(className: "dropdown-menu").ToList())
        {
            dropdown.AddToClassList("dropdown-hidden");
        }
    }
}