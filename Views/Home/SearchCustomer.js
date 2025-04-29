function searchCustomer() {
    let phone = document.getElementById("searchPhone").value;

    fetch(`/Home/SearchCustomer?phone=${phone}`)
        .then(response => response.json())
        .then(data => {
            if (data && !data.error) {
                document.getElementById("customerName").value = data.fullName;
                document.getElementById("customerPhone").value = data.phone;
            } else {
                alert("Customer not found!");
            }
        })
        .catch(error => {
            alert("Error searching for customer!");
        });
}
