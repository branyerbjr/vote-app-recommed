from flask import Flask, render_template, request, redirect, url_for, flash
import requests
from datetime import datetime, timezone

app = Flask(__name__)
app.secret_key = 'f9b8e7d4c3a2b1e0f9d8c7b6a5e4d3c2b1a0f9e8d7c6b5a4'  # Necesario para mensajes flash

# URL base de la API C# backend
API_BASE_URL = 'http://localhost:5254/api'

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/vote', methods=['GET', 'POST'])
def vote():
    if request.method == 'POST':
        choice = request.form.get('choice')
        timestamp_str = request.form.get('timestamp')

        if not choice or not timestamp_str:
            flash('Todos los campos son obligatorios.', 'danger')
            return redirect(url_for('vote'))

        # Convertir el timestamp a UTC
        try:
            # Parsear el timestamp recibido como datetime-local (sin zona horaria)
            local_dt = datetime.strptime(timestamp_str, '%Y-%m-%dT%H:%M')
            # Asumir que el tiempo local es UTC (si no, necesitas ajustar según la zona horaria)
            # Para convertir correctamente, podrías usar una librería como pytz o zoneinfo (Python 3.9+)
            utc_dt = local_dt.replace(tzinfo=timezone.utc)
            # Formatear en ISO 8601 con 'Z' para indicar UTC
            timestamp_iso = utc_dt.isoformat().replace('+00:00', 'Z')
        except ValueError:
            flash('Formato de fecha y hora inválido.', 'danger')
            return redirect(url_for('vote'))

        vote_data = {
            "Choice": choice,
            "Timestamp": timestamp_iso
        }

        try:
            response = requests.post(f'{API_BASE_URL}/vote', json=vote_data)
            if response.status_code == 200:
                flash('Voto enviado exitosamente.', 'success')
                return redirect(url_for('vote'))
            else:
                flash(f'Error al enviar el voto: {response.text}', 'danger')
        except requests.exceptions.RequestException as e:
            flash(f'Error de conexión: {e}', 'danger')

    return render_template('vote.html')

@app.route('/preferences', methods=['GET', 'POST'])
def preferences():
    if request.method == 'POST':
        user_id = request.form.get('user_id')
        item_a = request.form.get('item_a')
        item_b = request.form.get('item_b')
        item_c = request.form.get('item_c')

        if not user_id or not item_a or not item_b or not item_c:
            flash('Todos los campos son obligatorios.', 'danger')
            return redirect(url_for('preferences'))

        preferences_data = {
            "UserId": user_id,
            "Preferences": {
                "ItemA": int(item_a),
                "ItemB": int(item_b),
                "ItemC": int(item_c)
            }
        }

        try:
            response = requests.post(f'{API_BASE_URL}/preferences', json=preferences_data)
            if response.status_code == 200:
                flash('Preferencias guardadas exitosamente.', 'success')
                return redirect(url_for('preferences'))
            else:
                flash(f'Error al guardar preferencias: {response.text}', 'danger')
        except requests.exceptions.RequestException as e:
            flash(f'Error de conexión: {e}', 'danger')

    return render_template('preferences.html')

@app.route('/recommendations/<user_id>')
def recommendations(user_id):
    try:
        response = requests.get(f'{API_BASE_URL}/preferences/{user_id}')
        if response.status_code == 200:
            neighbors = response.json()
            return render_template('recommendations.html', user_id=user_id, neighbors=neighbors)
        elif response.status_code == 404:
            flash('No se encontraron preferencias para el usuario especificado.', 'warning')
            return redirect(url_for('index'))
        else:
            flash(f'Error al obtener recomendaciones: {response.text}', 'danger')
            return redirect(url_for('index'))
    except requests.exceptions.RequestException as e:
        flash(f'Error de conexión: {e}', 'danger')
        return redirect(url_for('index'))

if __name__ == '__main__':
    app.run(debug=True, port=5001)
