[Setup]
#ifndef AppVersion
  #define AppVersion "1.1.0"
#endif
AppName=ElectroObraApp
AppVersion={#AppVersion}
AppPublisher=ElectroObra Software
DefaultDirName={autopf}\ElectroObraApp
DefaultGroupName=ElectroObra App
OutputBaseFilename=ElectroObraApp_Setup
Compression=lzma
SolidCompression=yes
SetupIconFile=ElectroObraApp\Assets\electro-obra-logo.ico
WizardImageFile=ElectroObraApp\Assets\Images\electro-obra.png
WizardSmallImageFile=ElectroObraApp\Assets\Images\electro-obra.png
UninstallDisplayIcon={app}\electro-obra-logo.ico
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Tasks]
Name: "enable_seed"; Description: "Habilitar Datos Semilla (Testing)"; GroupDescription: "Configuración Avanzada:"; Flags: unchecked

[Files]
Source: "publish_win\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
; Copiamos el archivo de configuración base si no existe
Source: "ElectroObraApp\appsettings.json"; DestDir: "{commonappdata}\ElectroObraApp"; Flags: onlyifdoesntexist
Source: "ElectroObraApp\Assets\electro-obra-logo.ico"; DestDir: "{app}"; Flags: ignoreversion

[Dirs]
Name: "{commonappdata}\ElectroObraApp"; Permissions: everyone-modify

[Icons]
Name: "{group}\ElectroObra App"; Filename: "{app}\ElectroObraApp.Desktop.exe"; WorkingDir: "{app}"; IconFilename: "{app}\electro-obra-logo.ico"
Name: "{commondesktop}\ElectroObra App"; Filename: "{app}\ElectroObraApp.Desktop.exe"; WorkingDir: "{app}"; IconFilename: "{app}\electro-obra-logo.ico"

[Run]
Filename: "{app}\ElectroObraApp.Desktop.exe"; Description: "Lanzar aplicación"; Flags: nowait postinstall skipifsilent; WorkingDir: "{app}"

[Code]
var
  DeleteDatabase: Boolean;

procedure CurStepChanged(CurStep: TSetupStep);
var
  FilePath: String;
  FileContent: AnsiString;
  ContentStr: String;
begin
  if CurStep = ssPostInstall then
  begin
    FilePath := ExpandConstant('{commonappdata}\ElectroObraApp\appsettings.json');
    if FileExists(FilePath) then
    begin
      if LoadStringFromFile(FilePath, FileContent) then
      begin
        ContentStr := String(FileContent);
        if WizardIsTaskSelected('enable_seed') then
        begin
          Log('Configurando datos semilla como habilitados.');
          StringChange(ContentStr, '"SeedEnabled": false', '"SeedEnabled": true');
        end
        else
        begin
          StringChange(ContentStr, '"SeedEnabled": true', '"SeedEnabled": false');
        end;
        SaveStringToFile(FilePath, AnsiString(ContentStr), False);
      end;
    end;
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if (CurPageID = wpSelectTasks) and WizardIsTaskSelected('enable_seed') then
  begin
    if MsgBox('ATENCIÓN: La opción de Datos Semilla es solo para pruebas.' + #13#10 +
              'Se recomienda NO usarla en producción ya que poblará la base de datos con datos de ejemplo.' + #13#10#13#10 +
              '¿Está seguro de que desea continuar con esta opción activada?', mbConfirmation, MB_YESNO) = IDNO then
    begin
      Result := False;
    end;
  end;
end;

function ConfirmarEliminacion(): Boolean;
var
  Form: TForm;
  OKButton, CancelButton: TButton;
  Edit: TEdit;
  PromptLabel: TLabel;
begin
  Result := False;
  Form := TForm.Create(nil);
  try
    Form.ClientWidth := 300;
    Form.ClientHeight := 150;
    Form.Caption := 'Confirmación de Seguridad';
    Form.Position := poScreenCenter;

    PromptLabel := TLabel.Create(Form);
    PromptLabel.Parent := Form;
    PromptLabel.Caption := 'Para eliminar la base de datos, escriba "ELIMINAR" a continuación:';
    PromptLabel.Left := 10;
    PromptLabel.Top := 10;
    PromptLabel.Width := 280;
    PromptLabel.WordWrap := True;

    Edit := TEdit.Create(Form);
    Edit.Parent := Form;
    Edit.Left := 10;
    Edit.Top := 60;
    Edit.Width := 260;

    OKButton := TButton.Create(Form);
    OKButton.Parent := Form;
    OKButton.Caption := 'Aceptar';
    OKButton.Default := True;
    OKButton.ModalResult := mrOk;
    OKButton.Left := 130;
    OKButton.Top := 110;

    CancelButton := TButton.Create(Form);
    CancelButton.Parent := Form;
    CancelButton.Caption := 'Cancelar';
    CancelButton.Cancel := True;
    CancelButton.ModalResult := mrCancel;
    CancelButton.Left := 215;
    CancelButton.Top := 110;

    if Form.ShowModal() = mrOk then
    begin
      Result := (UpperCase(Trim(Edit.Text)) = 'ELIMINAR');
    end;
  finally
    Form.Free();
  end;
end;

function InitializeUninstall(): Boolean;
var
  DBPath: String;
begin
  Result := True;
  DeleteDatabase := False;
  DBPath := ExpandConstant('{commonappdata}\ElectroObraApp\ElectroObraApp.db');

  if FileExists(DBPath) then
  begin
    if MsgBox('Se ha detectado una base de datos. ¿Desea ELIMINARLA? Se recomienda realizar un backup.', mbConfirmation, MB_YESNO) = IDYES then
    begin
      if ConfirmarEliminacion() then
      begin
        DeleteDatabase := True;
      end
      else
      begin
        MsgBox('Confirmación incorrecta. La base de datos no será eliminada.', mbInformation, MB_OK);
      end;
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  DBPath: String;
  ConfigPath: String;
  Dir: String;
begin
  if (CurUninstallStep = usPostUninstall) and DeleteDatabase then
  begin
    DBPath := ExpandConstant('{commonappdata}\ElectroObraApp\ElectroObraApp.db');
    ConfigPath := ExpandConstant('{commonappdata}\ElectroObraApp\appsettings.json');
    Dir := ExpandConstant('{commonappdata}\ElectroObraApp');

    if FileExists(DBPath) then DeleteFile(DBPath);
    if FileExists(ConfigPath) then DeleteFile(ConfigPath);
    if DirExists(Dir) then RemoveDir(Dir);
  end;
end;