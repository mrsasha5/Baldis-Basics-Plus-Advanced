using PlusLevelStudio.Editor.Tools;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Tools
{
    internal class IndependentInfoPosterTool : PosterTool
    {

        private string _titleKey;

        private string _descKey;

        public override string titleKey => _titleKey;

        public override string descKey => _descKey;

        public IndependentInfoPosterTool(string type, string titleKey, string descKey) : base(type)
        {
            _titleKey = titleKey;
            _descKey = descKey;
        }
    }
}
