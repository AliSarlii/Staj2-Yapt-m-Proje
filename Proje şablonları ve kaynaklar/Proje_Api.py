from flask import Flask, request, jsonify
import pyodbc
import pandas as pd 
from sklearn.compose import ColumnTransformer 
from sklearn.preprocessing import LabelEncoder,OneHotEncoder  
from sklearn.model_selection import train_test_split 
from sklearn.svm import SVC 
from sklearn.metrics import accuracy_score
app = Flask(__name__)

# Flask ile api kurup http://127.0.0.1:5000/data adresinden veri bekliyoruz
@app.route('/data', methods=['POST'])
def receive_data():
    dataa = request.json  
    testVeriPred = 0
    accuracy = 0.0
    if not dataa:
        return jsonify({"error": "No data provided"}), 400
    
    # Veritabanı bağlantısını oluşturdum
    connection_string = (
    f'DRIVER={{ODBC Driver 17 for SQL Server}};'
    f'SERVER=DESKTOP-T4ANP1D;'
    f'DATABASE=Hastane_Proje;'
    f'Trusted_Connection=yes;'
    )
    try:
        # Sql sorgusu ile websitemizin bağlı olduğu databaseye bağlanıp Tiroid_Verileri tablosundan verileri çekiyoruz
        connection = pyodbc.connect(connection_string)
        cursor = connection.cursor()
        query = 'SELECT * FROM Tiroid_Verileri' 
        cursor.execute(query)
        data = cursor.fetchall()

        # Veri setini 2 boyutlu bir liste haline getiriyoruz
        for i in range(len(data)):
            data[i] = list(data[i])

        # Çektiğimiz veriyi DataFrame'ye dönüştürüyoruz
        df_columns = ['Id','Age', 'Gender', 'Smoking' ,'Hx_Smoking' ,'Hx_Radiothreapy' , 'Thyroid_Function' , 'Physical_Examination' , 'Adenopathy' , 'Pathology' ,'Focality' , 'Risk' , 'T' , 'N' , 'M' , 'Stage' , 'Response' , 'Recurred' , "Olcum_Tarihi" , "Kullanici"]
        df = pd.DataFrame(data, columns=df_columns)

        df = df.dropna(subset=['Recurred'])

        # Target verisini çıkarıyouz
        y = df["Recurred"]

        # Makine öğrenmesinde işimize yaramayacak kolonları veri setimizden çıkarıyoruz
        df = df.drop('Recurred', axis=1)
        df = df.drop("Id" , axis=1)
        df = df.drop("Kullanici" , axis=1)
        df = df.drop("Olcum_Tarihi" , axis=1)

        # Api'den çektiğimiz veriyi makine öğrenmesine sokabilmemiz için SQL Server'dan çektiğimiz veri ile beraber encode olması lazım.
        # Beraber encode edebilmemiz için de Api'den gelen veri ile SQL Server'dan gelen veriyi birleştireceğim. Bunun için de 
        # önce Api'den gelen veriyi Dataframe'ye dönüştürüyorum
        testveri_columns = ['Age', 'Gender', 'Smoking' ,'Hx_Smoking' ,'Hx_Radiothreapy' , 'Thyroid_Function' , 'Physical_Examination' , 'Adenopathy' , 'Pathology' ,'Focality' , 'Risk' , 'T' , 'N' , 'M' , 'Stage' , 'Response']

        print(dataa)    
        data_df = [dataa]
        test_df = pd.DataFrame(data_df, columns=testveri_columns)

        # Verileri birleştiriyorum
        merged_df = pd.concat([df, test_df], axis=0, ignore_index=True)
        df = merged_df

        # Birleşmiş olan verimi OneHotEncoder kullanarak encode ediyorum
        columns_to_encode = ['Thyroid_Function', 'Physical_Examination', 'Adenopathy', 'Pathology',
            'Focality', 'Risk', 'T', 'N', 'M', 'Stage', 'Response']
        ct = ColumnTransformer([('onehot', OneHotEncoder(), columns_to_encode)], remainder='passthrough')
        data_encoded = ct.fit_transform(df)
        x = data_encoded
        x = pd.DataFrame(x)

        # Encode olmuş verimden tekrardan Api'den gelen veriyi çıkarıyorum
        testveridonusum = pd.DataFrame(x.iloc[-1]).transpose()
        x.drop([len(x)-1], axis=0, inplace=True)

        # SQL Server'dan gelen verimizi ve Target verimizi train ve test olmak üzere model eğitmek için bölüyorum
        X_train, X_test, y_train, y_test = train_test_split(x, y, test_size=0.3, random_state=42)

        # Train verilerimizle modelimizi eğitiyorum
        svm_model = SVC(kernel='linear', random_state=42)
        svm_model.fit(X_train, y_train)

        # Ayırdığımız test verileri ile de modelimizin doğruluğunu ölçüyorum
        y_pred = svm_model.predict(X_test)
        accuracy = accuracy_score(y_test, y_pred)
        print("Model Doğruluğu:", accuracy)

        # Api'den gelen verinin de model tahminini alıyorum
        testVeriPred=svm_model.predict(testveridonusum)
        print("Test verisinin sonucu : ",testVeriPred)


    except Exception as e:
        print(f"Hata: {e}")
    finally:
        connection.close()

    # Sonuç verisinin JSON formatına dönüşebilmesi için listeye dönüştürüyorum
    testVeriPred_list = testVeriPred.tolist()

    # Sonuç verisi kullanıcıya gidiyor
    return jsonify({'testVeriPred_list': testVeriPred_list, 'accuracy': accuracy}), 200
if __name__ == '__main__':
    app.run(debug=True)
