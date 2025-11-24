using Default;

namespace Apis.UI.Focus
{
    public interface IControlTab : IController
    {
        public void Change(int id);
        public void OnOpen();
        public void OnClose();
    }

    public class UI_MultiFuncTab : UI_Base, IController
    {
        public virtual void Change(int id)
        {
            
        }

        public virtual void KeyControl()
        {
            
        }
        
        public virtual void GamePadControl()
        {
            
        }
    }
    public class UI_FocusContent: UI_Base, IController
    {

        public virtual void KeyControl()
        {
            
        }

        public virtual void GamePadControl()
        {
            
        }

        public virtual void OnOpen()
        {
            
        }

        public virtual void OnClose()
        {
            
        }
    }
}