namespace CutEditor.Views;

using System.Windows.Controls;
using CutEditor.ViewModel;
using Shared.Templet.TempletTypes;

public sealed partial class PgSkillDetail : Page
{
    public PgSkillDetail(SkillTemplet skillTemplet)
    {
        this.InitializeComponent();
        this.DataContext = new VmSkillDetail(skillTemplet);
    }
}
