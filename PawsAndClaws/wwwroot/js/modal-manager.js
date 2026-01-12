/**
 * Modal Manager Controller
 * Reusable notification and confirmation modal for displaying dynamic messages across the application
 */
const ModalManager = {
  /**
   * Shows the modular notification modal
   * @param {string} title - The bold heading
   * @param {string} description - The body text
   * @param {string} btnText - Text for the action button (default: "OKAY")
   * @param {function} callback - Function to run when closed (optional)
   */
  show: function (title, description, btnText = "OKAY", callback = null) {
    const modal = document.getElementById("notificationModal");
    const titleEl = document.getElementById("modalTitle");
    const descEl = document.getElementById("modalDescription");
    const btn = document.getElementById("modalActionBtn");
    const actionContainer = document.getElementById("modalActionContainer");

    // Set Content
    titleEl.innerText = title;
    descEl.innerText = description;
    btn.innerText = btnText;

    // Reset and Apply Callback
    btn.onclick = () => {
      this.hide();
      if (callback) callback();
    };

    // Show only action button
    btn.classList.remove("hidden");
    if (actionContainer) {
      actionContainer.innerHTML = "";
      actionContainer.appendChild(btn);
    }

    // Show Modal
    modal.classList.remove("hidden");
    document.body.style.overflow = "hidden"; // Prevent background scrolling
  },

  /**
   * Shows a confirmation dialog with Yes/No options
   * @param {string} title - The bold heading
   * @param {string} description - The body text
   * @param {string} confirmText - Text for the confirm button (default: "YES")
   * @param {string} cancelText - Text for the cancel button (default: "NO")
   * @param {function} onConfirm - Function to run when user confirms (optional)
   * @param {function} onCancel - Function to run when user cancels (optional)
   */
  confirm: function (
    title,
    description,
    confirmText = "YES",
    cancelText = "NO",
    onConfirm = null,
    onCancel = null
  ) {
    const modal = document.getElementById("notificationModal");
    const titleEl = document.getElementById("modalTitle");
    const descEl = document.getElementById("modalDescription");
    const actionContainer = document.getElementById("modalActionContainer");

    // Set Content
    titleEl.innerText = title;
    descEl.innerText = description;

    // Clear and build action buttons
    actionContainer.innerHTML = "";

    // Cancel Button
    const cancelBtn = document.createElement("button");
    cancelBtn.type = "button";
    cancelBtn.className =
      "px-8 py-3.5 rounded-full text-xs font-black uppercase tracking-widest border-2 border-gray-200 text-gray-600 hover:border-gray-300 hover:bg-gray-50 transition-all";
    cancelBtn.innerText = cancelText;
    cancelBtn.onclick = () => {
      this.hide();
      if (onCancel) onCancel();
    };

    // Confirm Button
    const confirmBtn = document.createElement("button");
    confirmBtn.type = "button";
    confirmBtn.className =
      "bg-[#F87004] text-white px-8 py-3.5 rounded-full text-xs font-black uppercase tracking-widest shadow-lg shadow-orange-100 hover:scale-[1.03] active:scale-[0.98] transition-all";
    confirmBtn.innerText = confirmText;
    confirmBtn.onclick = () => {
      this.hide();
      if (onConfirm) onConfirm();
    };

    actionContainer.appendChild(cancelBtn);
    actionContainer.appendChild(confirmBtn);

    // Show Modal
    modal.classList.remove("hidden");
    document.body.style.overflow = "hidden"; // Prevent background scrolling
  },

  /**
   * Hides the notification modal and restores background scrolling
   */
  hide: function () {
    const modal = document.getElementById("notificationModal");
    modal.classList.add("hidden");
    document.body.style.overflow = ""; // Restore scrolling
  },
};
