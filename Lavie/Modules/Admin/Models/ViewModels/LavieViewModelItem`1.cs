namespace Lavie.Modules.Admin.Models.ViewModels
{
    public class LavieViewModelItem<T> : LavieViewModel
    {
        #region Constructors

        public LavieViewModelItem()
        {
            this.Item = default(T);
        }
        /// <summary>
        /// Initializes a new instance of the LavieViewModelItem class.
        /// </summary>
        /// <param name="item">Object that the class is holding.</param>
        public LavieViewModelItem(T item)
        {
            this.Item = item;
        }

        #endregion

        /// <summary>
        /// Object that the class is holding.
        /// </summary>
        public T Item { get; set; }
    }
}
