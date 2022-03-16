using Library.Engine.Types;
using Library.Types;
using ScenarioEdit.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ScenarioEdit
{
    public partial class FormEditEffects : Form
    {
        public EngineCore Core { get; set; }

        public List<MetaEffect> Effects { get; set; }

        public FormEditEffects(EngineCore core, List<MetaEffect> effects)
        {
            InitializeComponent();
            Effects = effects;

            if (Effects == null)
            {
                Effects = new List<MetaEffect>();
            }

            Core = core;
        }

        public FormEditEffects()
        {
            InitializeComponent();
        }

        private void FormEditSpawner_Load(object sender, EventArgs e)
        {
            this.AcceptButton = buttonSave;
            this.CancelButton = buttonCancel;

            listViewContainer.Items.Add(new ListViewItem(new string[] { "Speed", Effects.Where(o => o.EffectType == ItemEffect.Speed).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.Speed });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Strength", Effects.Where(o => o.EffectType == ItemEffect.Strength).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.Strength });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Dexterity", Effects.Where(o => o.EffectType == ItemEffect.Dexterity).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.Dexterity });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Constitution", Effects.Where(o => o.EffectType == ItemEffect.Constitution).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.Constitution });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Armor Class", Effects.Where(o => o.EffectType == ItemEffect.ArmorClass).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.ArmorClass });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Intelligence", Effects.Where(o => o.EffectType == ItemEffect.Intelligence).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.Intelligence });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Code Resistance", Effects.Where(o => o.EffectType == ItemEffect.ColdResistance).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.ColdResistance });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Lightning Resistance", Effects.Where(o => o.EffectType == ItemEffect.LightningResistance).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.LightningResistance });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Fire Resistance", Effects.Where(o => o.EffectType == ItemEffect.FireResistance).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.FireResistance });
            listViewContainer.Items.Add(new ListViewItem(new string[] { "Earth Resistance", Effects.Where(o => o.EffectType == ItemEffect.EarthResistance).Sum(o => o.Value).ToString() }) { Tag = ItemEffect.EarthResistance });
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Effects.Clear();

            foreach (ListViewItem obj in listViewContainer.Items)
            {
                if (Int32.TryParse(obj.SubItems[1].Text, out int value))
                {
                    if (value != 0)
                    {
                        Effects.Add(new MetaEffect()
                        {
                            EffectType = (ItemEffect)obj.Tag,
                            ValueType = ItemEffectType.Fixed,
                            Value = value
                        });
                    }
                }
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void listViewContainer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = listViewContainer.GetItemAt(e.X, e.Y);

            using (var form = new FormEditInteger(item.Text, item.SubItems[1].Text))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (Int32.TryParse(form.PropertyValue, out int value))
                    {
                        item.SubItems[1].Text = form.PropertyValue;
                    }
                }
            }
        }
    }
}