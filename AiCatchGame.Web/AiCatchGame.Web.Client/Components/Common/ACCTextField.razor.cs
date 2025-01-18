using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace AiCatchGame.Web.Client.Components.Common
{
    public partial class ACCTextField
    {
        public string? _value;
        public bool Immediate { get; set; }

        [Parameter]
        public string? Label { get; set; }

        [Parameter]
        public EventCallback OnEnter { get; set; }

        [Parameter]
        public string? Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                {
                    return;
                }

                Task.Run(async() => await SetValueAsync(value, true, false));
            }
        }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        protected override Task OnInitializedAsync()
        {
            ValueChanged.InvokeAsync(Value);
            if (OnEnter.HasDelegate)
            {
                Immediate = true;
            }
            return base.OnInitializedAsync();
        }

        protected virtual async Task SetValueAsync(string? value, bool updateText = true, bool force = false)
        {
            if (EqualityComparer<string>.Default.Equals(Value, value) && !force)
            {
                return;
            }

            _value = value;

            await ValueChanged.InvokeAsync(Value);

            //if (updateText)
            //{
            //    await UpdateTextPropertyAsync(false);
            //}
        }

        private async void OnTextKeyDown(KeyboardEventArgs args)
        {
            if (OnEnter.HasDelegate && args.Key == "Enter")
            {
                await OnEnter.InvokeAsync();
            }
        }
    }
}