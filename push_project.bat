@echo off
title Push AR Assembly Guide to GitHub
echo ===================================================
echo   Envoi du projet AR-Assembly-Guide vers GitHub
echo ===================================================
echo.

echo [+] Verification du statut Git...
git status

echo.
echo [+] Ajout des fichiers au commit...
git add .

echo.
echo [+] Creation du commit...
git commit -m "Ajout du rapport LaTeX et mise a jour du projet"

echo.
echo [+] Envoi des fichiers vers le depot distant (push)...
echo Note : Une fenetre de connexion GitHub va apparaitre pour confirmer votre identite.
echo.
git push origin main

if %ERRORLEVEL% equ 0 (
    echo.
    echo ===================================================
    echo  [OK] Le projet a ete envoye avec succes sur GitHub !
    echo ===================================================
) else (
    echo.
    echo ===================================================
    echo  [ERREUR] Le push a echoue. Veuillez verifier vos identifiants.
    echo ===================================================
)

echo.
pause
