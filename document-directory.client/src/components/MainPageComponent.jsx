import {useEffect, useState} from 'react';
import { Input, Space, Button, DatePicker } from 'antd';
import { FolderFilled, FolderAddFilled, FileAddFilled, FileFilled, CloseOutlined } from '@ant-design/icons';
const { Search } = Input;
const { TextArea } = Input;

function MainPageComponent() {
    const [availableNodes, setAvailableNodes] = useState([]);
    const [hierarchy, setHierarchy] = useState('/');
    const [isDirectoryCreateFormVisible, setIsDirectoryCreateFormVisible] = useState(false);
    const [isDocumentCreateFormVisible, setIsDocumentCreateFormVisible] = useState(false);
    const [isShowInfoChosenDirectory, setIsShowInfoChosenDirectory] = useState(false);
    const [isShowInfoChosenDocument, setIsShowInfoChosenDocument] = useState(false);
    const [chosenNode, setChosenNode] = useState({})

    const onSearch = (value, _e, info) => console.log(info?.source, value);
    const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiUHJvc3RvIiwiSWQiOiIyIiwiZXhwIjoxNzIzNzEwNjEwLCJpc3MiOiJNeUF1dGhTZXJ2ZXIiLCJhdWQiOiJNeUF1dGhDbGllbnQifQ.QFYli_ldShOtf_zbdY3h9vq_zpEPTksiWQNR-F77njs"
    async function createDirectory() {
        const name = document.querySelector('#inputDirectoryName').value;
        const response = await fetch('https://localhost:7018/api/documents', {
            method: 'POST', 
            headers: new Headers({ "Content-Type": "application/json", "Authorization": "Bearer " + token }), 
            body: JSON.stringify({
                type: "Directory",
                name: name,
                content: "",
                activityEnd: "2024-08-14T04:50:29.980Z",
                folderId: 0
            })
            });
        if (response.status == 201)
        {
            const json = await response.json();
            setAvailableNodes([...availableNodes, json]);
            setIsDirectoryCreateFormVisible(false);
        }
    }

    function createDocument() {
        const name = document.querySelector('#inputDocumentName').value;

        setAvailableNodes([...availableNodes, {type: "Document", name: name != "" ? name : "без названия"}]);
        setIsDocumentCreateFormVisible(false);
    }

    function onChangeDateActivity(date, dateString) {
        console.log(date, dateString);
    }

    async function viewDirectoryContent(directory) {
        const response = await fetch(`api/NodeHierarchy/${directory.id}`);
        console.log(response);
    }

    async function getAvailableNodes() {
        let response = await fetch("api/documents/all");
        console.log(response.data);
    }

    useEffect(() => {
        // getAvailableNodes();
    }, [])

    return (
        <>
            {isDirectoryCreateFormVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название папки' id='inputDirectoryName' style={{width: '70%'}}/>
                            <CloseOutlined onClick={() => setIsDirectoryCreateFormVisible(false)}/>
                        </div>
                        <Button size='small' onClick={createDirectory} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            {isDocumentCreateFormVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название документа' id='inputDocumentName' style={{width: '70%'}}/>
                            <CloseOutlined onClick={() => setIsDocumentCreateFormVisible(false)}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>Дата окончания действия документа</p>
                            <DatePicker onChange={onChangeDateActivity} format={{format: 'DD-MM-YYYY'}} placeholder='Выберите дату' placement='bottomLeft'/>
                        </div>
                        <TextArea
                            placeholder="Введите содержимое документа"
                            autoSize={{
                                minRows: 6,
                                maxRows: 10
                            }}
                            id="inputDocumentContent"
                        />
                        <Button size='small' onClick={createDocument} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            <div>
                <Search
                    placeholder="Введите название документа или папки"
                    allowClear
                    onSearch={onSearch}
                    style={{
                        maxWidth: '700px',
                        width: '70%'
                    }}
                />
            </div>
            <br />
            <div style={{display: 'flex', justifyContent: 'space-between'}}>
                <div>
                    <button onClick={() => setIsDirectoryCreateFormVisible(true)} className="btnWithIcon">
                        <FolderAddFilled style={{fontSize: '30px'}}/>
                        <p style={{textWrap: 'wrap', fontSize: '14px'}}>Создать папку</p>
                    </button>
                    <button onClick={() => setIsDocumentCreateFormVisible(true)} className="btnWithIcon">
                        <FileAddFilled style={{fontSize: '30px'}}/>
                        <p style={{textWrap: 'wrap', fontSize: '14px'}}>Создать документ</p>
                    </button>
                </div>
                {isShowInfoChosenDirectory && !isShowInfoChosenDocument && <div>
                    <p>Имя папки: {chosenNode.name}</p>
                </div>}
                {isShowInfoChosenDocument && !isShowInfoChosenDirectory && <div>
                    <p>Имя документа: {chosenNode.name}</p>
                </div>}
            </div>
            <p style={{textAlign: 'left', margin: '10px 0'}}>{hierarchy}</p>
            <div>
                <Space wrap style={{gap: '15px'}}>
                    {availableNodes.map((node, index) => (
                        <div key={index} className='availableNode' style={{maxWidth: '120px'}}>
                            {node.type == "Directory" ? 
                            <button className="btnWithIcon" onClick={() => {setIsShowInfoChosenDocument(false); setChosenNode(node); setIsShowInfoChosenDirectory(true);}} onDoubleClick={() => viewDirectoryContent(node)}>
                                <FolderFilled style={{fontSize: '50px'}} />
                                <p style={{width: '110px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{node.name}</p>
                            </button>
                            :
                            <button className="btnWithIcon" onClick={() => {setIsShowInfoChosenDirectory(false); setChosenNode(node); setIsShowInfoChosenDocument(true);}}>
                                <FileFilled style={{fontSize: '45px'}}/>
                                <p style={{width: '110px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{node.name}</p> 
                            </button>
                            }
                        </div>
                    ))}
                </Space>
            </div>
            
        </>
    );
}

export default MainPageComponent;