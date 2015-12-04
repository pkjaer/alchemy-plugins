<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <title>Add User By Name</title>
    </head>
    <body>
        <div class="content">
            <h1>Add User By Name</h1>
            <div>
                <div class="label">Username (e.g. domain\name): <span class="required">*</span></div>
                <input id="Username" type="text" class="field" value="" tabindex="1"/>
                <div id="UserNameRequiredError" class="error" style="display:none">Username is required.</div>
            </div>
            <div>
                <div class="label">Full name:</div>
                <input id="FullName" type="text" class="field" value="" tabindex="1"/>
            </div>
            <div class="checkboxes">
                <input type="checkbox" id="OpenAfterwards" checked="checked" tabindex="1" />
                <label for="OpenAfterwards">Edit the user after saving.</label>
            </div>
            <div class="buttons right">
                <c:Button ID="SaveButton" runat="server" Label="Save" TabIndex="1" class="button2013 green tridion button" />
                <c:Button ID="CancelButton" runat="server" Label="Cancel" TabIndex="1" class="button2013 gray tridion button" />
            </div>
        </div>
    </body>
</html>