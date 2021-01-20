using System;
using System.Linq;
using Android.Graphics.Drawables;
using Android.Util;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public abstract class SelectableViewHolder : RecyclerView.ViewHolder, global::Android.Views.View.IOnClickListener
	{
		bool _isSelected;
		Drawable _selectedDrawable;
		Drawable _selectableItemDrawable;
		readonly bool _isSelectionEnabled;

		protected SelectableViewHolder(global::Android.Views.View itemView, bool isSelectionEnabled = true) : base(itemView)
		{
			itemView.SetOnClickListener(this);
			_isSelectionEnabled = isSelectionEnabled;
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;

				SetSelectionStates(_isSelected);

				ItemView.Activated = _isSelected;
				OnSelectedChanged();
			}
		}

		protected virtual bool UseDefaultSelectionColor => true;

		public void OnClick(global::Android.Views.View view)
		{
			if (_isSelectionEnabled)
			{
				OnViewHolderClicked(AdapterPosition);
			}
		}

		public event EventHandler<int> Clicked;

		protected virtual void OnSelectedChanged()
		{
		}

		protected virtual void OnViewHolderClicked(int adapterPosition)
		{
			Clicked?.Invoke(this, adapterPosition);
		}

		void SetSelectionStates(bool isSelected)
		{
			if (!UseDefaultSelectionColor)
			{
				return;
			}

			if (Forms.IsMarshmallowOrNewer)
			{
				// We're looking for the foreground ripple effect, which is not available on older APIs
				// Limiting this to Marshmallow and newer, because View.setForeground() is not available on lower APIs
				_selectableItemDrawable = !isSelected ? null : (_selectableItemDrawable ?? GetSelectableItemDrawable());

				ItemView.Foreground = _selectableItemDrawable;
			}

			_selectedDrawable = !isSelected ? null : (_selectedDrawable ?? GetSelectedDrawable());

			ItemView.Background = _selectedDrawable;
		}

		Drawable GetSelectedDrawable()
		{
			using (var value = new TypedValue())
			{
				var context = ItemView.Context;

				if (!context.Theme.ResolveAttribute(global::Android.Resource.Attribute.ColorActivatedHighlight, value, true))
				{
					return null;
				}

				var color = Color.FromUint((uint)value.Data);
				var colorDrawable = new ColorDrawable(color.ToAndroid());

				var stateListDrawable = new StateListDrawable();
				stateListDrawable.AddState(new[] { global::Android.Resource.Attribute.StateActivated }, colorDrawable);
				stateListDrawable.AddState(StateSet.WildCard.ToArray(), null);

				return stateListDrawable;
			}
		}

		Drawable GetSelectableItemDrawable()
		{
			using (var value = new TypedValue())
			{
				var context = ItemView.Context;

				context.Theme.ResolveAttribute(global::Android.Resource.Attribute.SelectableItemBackground, value, true);

				return ContextCompat.GetDrawable(context, value.ResourceId);
			}
		}
	}
}