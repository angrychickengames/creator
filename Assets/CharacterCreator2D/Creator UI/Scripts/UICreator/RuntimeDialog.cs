using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
	public class RuntimeDialog : MonoBehaviour 
	{
        /// <summary>
        /// UI Text displaying the title.
        /// </summary>
        [Tooltip("UI Text displaying the title")]
		public Text title;

        /// <summary>
        /// UI Text displaying the message.
        /// </summary>
        [Tooltip("UI Text displaying the message")]
		public Text message;
        
        /// <summary>
        /// Display RuntimeDialog.
        /// </summary>
        /// <param name="title">Title to display.</param>
        /// <param name="message">Message to display</param>
		public void Show(string title, string message)
		{
			this.title.text = title;
			this.message.text = message;
			this.gameObject.SetActive (true);
		}

        /// <summary>
        /// Close RuntimeDialog.
        /// </summary>
		public void Close()
		{
			this.gameObject.SetActive (false);
		}
	}
}