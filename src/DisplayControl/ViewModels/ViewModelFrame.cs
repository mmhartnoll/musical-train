namespace DisplayControl.ViewModels
{
    internal class ViewModelFrame : ViewModel
    {
        public ViewModel Current { get; }
        public ViewModel Previous { get; }

        public ViewModelFrame(ViewModel current, ViewModel previous, bool preventHidingDialog = true)
        {
            if (preventHidingDialog && previous is DialogViewModel)
            {
                Current = previous;
                Previous = current;
            }
            else
            {
                Current = current;
                Previous = previous;
            }
        }
    }
}