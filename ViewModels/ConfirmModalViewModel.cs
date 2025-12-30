public class ConfirmModalViewModel
{
    public string ModalId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string BodyHtml { get; set; } = default!;
    public string ConfirmAction { get; set; } = default!;
    public string Controller { get; set; } = default!;
    public int EntityId { get; set; }
    public string ConfirmButtonClass { get; set; } = "btn-danger";
}
